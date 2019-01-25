namespace IdentityAndAcccess.Workflow.AddGroupToGroupApiTypes

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.CommonDomainTypes

open System
open IdentityAndAcccess.DomainTypes
open IdentityAndAccess.DatabaseTypes
open IdentityAndAcccess.DomainServicesImplementations
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.Functions








///Add group to group worflow types
/// 
/// 


//add group to group workflow input types
type UnvalidatedGroupIds = {
        GroupIdToAddTo : string
        GroupIdToAdd: string
    }



//Defining the comand types
type Command<'data> = {
    Data : 'data;
    TimeStamp : DateTime;
    UserId : string;
}




type AddGroupToGroupCommand =
        Command<UnvalidatedGroupIds> 


///Ouputs of the add group to group worflow 
//- Sucess types




type GroupAddedToGroupEvent = { 
    GroupMemberAdded : Dto.GroupMember
    }


//- Failure types

type AddGroupToGroupError = 
    | ValidationError of string
    | AddError of string
    | DbError of string


//Worflow type 

type AddGroupToGroupWorkflow = 
    Group.Group -> Group.Group -> Result<GroupAddedToGroupEvent , AddGroupToGroupError>
