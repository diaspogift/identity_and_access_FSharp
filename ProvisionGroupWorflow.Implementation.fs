module IdentityAndAcccess.Workflow.ProvisionGroupApiTypes.ProvisionGroupWorflowImplementation


open IdentityAndAcccess

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainServicesImplementations.Tenant
open IdentityAndAcccess.Workflow.ProvisionGroupApiTypes
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes
open IdentityAndAcccess.DomainTypes.Functions
open System.Text.RegularExpressions
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainServicesImplementations
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainTypes.Functions.Dto
open IdentityAndAcccess.DomainTypes.Functions.Dto
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainServicesImplementations














// Step 1 Validation  


type ValidatedGroup = {
    TenantId : CommonDomainTypes.TenantId
    Name : GroupName
    Description : GroupDescription
    Members : GroupMember  array
    }
 


type ValidateGroup =

    UnvalidatedGroup -> Result<ValidatedGroup, ProvisionGroupError>







//Step 2 - Create group 

type CreateGroup = 

    Tenant.Tenant -> ValidatedGroup -> Result<Group.Group, ProvisionGroupError>



//Step 3 - Create events step

type CreateEvents = Group.Group -> GroupProvisionedEvent






///Step 1 validate tenant provision impl
let validateGroup : ValidateGroup =

    fun aUnvalidatedGroup->

        result {


            let! tenantId = 
                aUnvalidatedGroup.TenantId
                |> TenantId.create' 
                |> Result.mapError ProvisionGroupError.ValidationError

            let! name = 
                aUnvalidatedGroup.Name
                |> GroupName.create' 
                |> Result.mapError ProvisionGroupError.ValidationError

            let! description = 
                aUnvalidatedGroup.Description 
                |> GroupDescription.create' 
                |> Result.mapError ProvisionGroupError.ValidationError

            let validatedGroup:ValidatedGroup = {
                TenantId = tenantId
                Name = name
                Description = description
                Members = [||]
                } 

            return validatedGroup

            }





///Step2 create group impl
let create : CreateGroup = 

    fun tenant tvalidatedGroup ->
       Tenant.provisionGroup tenant tvalidatedGroup.Name tvalidatedGroup.Description
       |> Result.mapError ProvisionGroupError.CreateError

        


///Step4 create events impl
let createEvents : CreateEvents = 

    fun aGroup ->

       let groupDto:Dto.Group = aGroup |> Group.fromDomain
       let ug = match groupDto with 
                | Group.Standard s -> s
                | Group.Internal i -> i

       let groupCreatedEvent : GroupProvisionedEvent = {
            GroupId = ug.GroupId
            TenantId = ug.TenantId
            Group = ug
            }

       groupCreatedEvent

        
                    

 



















///Create or provosion group workflow implementation
/// 
/// 
let provisionGroupWorkflow: ProvisionGroupWorkflow = 

    fun tenant unvalidatedGroup ->

        let create = tenant |> create 
        let create = Result.bind create
        let createEvents = Result.map createEvents


        unvalidatedGroup
        |> validateGroup
        |> create
        |> createEvents
        







 
