module IdentityAndAcccess.Workflow.ProvisionRoleApiTypes.ProvisionRoleWorflowImplementation

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainServicesImplementations.Tenant
open IdentityAndAcccess.Workflow.ProvisionRoleApiTypes
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes
open IdentityAndAcccess.DomainTypes.Functions
open System.Text.RegularExpressions
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainServicesImplementations
open IdentityAndAcccess.DomainTypes
open IdentityAndAccess.DatabaseTypes















// Step 1 Validation  


type ValidatedRole = {
    TenantId : TenantId
    Name: RoleName
    Description: RoleDescription
    }
 


type ValidateRole =

    UnvalidatedRole-> Result<ValidatedRole, ProvisionRoleError>







//Step 2 - Create role 

type CreateRole = 

    Tenant -> ValidatedRole -> Result<Role, ProvisionRoleError>



//Step 3 - Create events step

type CreateEvents = Role -> RoleProvisionedEvent






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
            Role = (aRole |> DbHelpers.fromRoleDomainToDto)
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
        







 
