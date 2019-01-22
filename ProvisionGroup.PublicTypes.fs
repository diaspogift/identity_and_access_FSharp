namespace IdentityAndAcccess.Workflow.ProvisionGroupApiTypes

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.CommonDomainTypes

open System
open IdentityAndAcccess.DomainTypes
open IdentityAndAccess.DatabaseTypes
open IdentityAndAcccess.DomainServices








///Provision group worflow types
/// 
/// 


//Provision group workflow input types
type UnvalidatedGroup = {
    TenantId : string
    Name : string
    Description : string
    Members : UnvalidatedGroupMember  array
    }
    and UnvalidatedGroupMember = {
            MemberId : string
            TenantId :  string
            Name: string
            Type : string
            }




//Defining the comand types
type Command<'data> = {
    Data : 'data;
    TimeStamp : DateTime;
    UserId : string;
}




type ProvisionGroupCommand =
        Command<UnvalidatedGroup> 


///Ouputs of the provisioned group worflow 
//- Sucess types




type GroupProvisionedEvent = { 
    Group : GroupDto
    }


//- Failure types

type ProvisionGroupError = 
    | ValidationError of string
    | CreateError of string
    | DbError of string


//Worflow type 

type ProvisionGroupWorkflow = 
    Tenant -> UnvalidatedGroup -> Result<GroupProvisionedEvent , ProvisionGroupError>
