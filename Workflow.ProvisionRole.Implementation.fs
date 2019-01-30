namespace IdentityAndAcccess.Workflow.ProvisionRoleApiTypes

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces
open IdentityAndAcccess.DomainTypes.Functions
open System.Text.RegularExpressions
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes















// Step 1 Validation  






///Provision role worflow types
/// 
/// 


//Provision role workflow input types
type UnvalidatedRole = {
        TenantId : string
        Name: string
        Description: string
    }





type RoleProvisionedEvent = { 
    Role : Dto.Role
    }


//- Failure types

type ProvisionRoleError = 
    | ValidationError of string
    | CreateError of string
    | DbError of string


//Worflow type 

type ProvisionRoleWorkflow = 
    Tenant -> UnvalidatedRole -> Result<RoleProvisionedEvent , ProvisionRoleError>



type ValidatedRole = {
    TenantId : TenantId
    Name: RoleName
    Description: RoleDescription
    }
 


type ValidateRole =

    UnvalidatedRole-> Result<ValidatedRole, ProvisionRoleError>







//Step 2 - Create role 

type CreateRole = 

    Tenant.Tenant -> ValidatedRole -> Result<Role.Role, ProvisionRoleError>



//Step 3 - Create events step

type CreateEvents = Role.Role -> RoleProvisionedEvent



module ProvisionRoleWorflowImplementation =


    ///Step 1 validate role provision impl
    let validateRole : ValidateRole =

        fun aUnvalidatedRole->

            result {


                let! tenantId = 
                    aUnvalidatedRole.TenantId
                    |> TenantId.create' 
                    |> Result.mapError ProvisionRoleError.ValidationError

                let! name = 
                    aUnvalidatedRole.Name
                    |> RoleName.create' 
                    |> Result.mapError ProvisionRoleError.ValidationError

                let! description = 
                    aUnvalidatedRole.Description 
                    |> RoleDescription.create' 
                    |> Result.mapError ProvisionRoleError.ValidationError

                    
                let validatedGroup:ValidatedRole = {
                    TenantId = tenantId
                    Name = name
                    Description = description
                    } 

                return validatedGroup

                }





    ///Step2 create role impl
    let create : CreateRole = 

        fun tenant validatedRole ->

           Tenant.provisionRole tenant validatedRole.Name validatedRole.Description
           |> Result.mapError ProvisionRoleError.CreateError

            


    ///Step4 create events impl
    let createEvents : CreateEvents = 

        fun aRole ->

           let roleCreatedEvent : RoleProvisionedEvent = {
                Role = aRole |> Dto.Role.fromDomain
           }
           roleCreatedEvent

            
                        

     







    ///Create or provosion role workflow implementation
    /// 
    /// 
    let provisionRoleWorkflow: ProvisionRoleWorkflow = 

        fun tenant unvalidatedRole ->

            let create = tenant |> create 
            let create = Result.bind create
            let createEvents = Result.map createEvents


            unvalidatedRole
            |> validateRole
            |> create
            |> createEvents
            







     
