namespace IdentityAndAcccess.Workflow.ProvisionRoleApiTypes

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.CommonDomainTypes

open System
open IdentityAndAcccess.DomainTypes

open IdentityAndAcccess.DomainTypes.Functions








///Provision role worflow types
/// 
/// 


//Provision role workflow input types
type UnvalidatedRole = {
        TenantId : string
        Name: string
        Description: string
    }



//Defining the comand types
type Command<'data> = {
    Data : 'data;
    TimeStamp : DateTime;
    UserId : string;
}




type ProvisionRoleCommand =
        Command<UnvalidatedRole> 


///Ouputs of the provisioned role worflow 
//- Sucess types




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
