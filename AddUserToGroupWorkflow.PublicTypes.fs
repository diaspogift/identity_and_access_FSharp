namespace IdentityAndAcccess.Workflow.AddUserToGroupApiTypes

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.CommonDomainTypes

open System
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.Functions








///Add user to group worflow types
/// 
/// 


//add user to group workflow input types
type UnvalidatedGroupAndUserId = {
        GroupId : string
        UserId: string
    }



//Defining the comand types
type Command<'data> = {
    Data : 'data;
    TimeStamp : DateTime;
    UserId : string;
}




type AddUserToGroupCommand =
        Command<UnvalidatedGroupAndUserId> 


///Ouputs of the add user to group worflow 
//- Sucess types




type UserAddedToGroupEvent = { 
    GroupMemberAdded : Dto.GroupMember
    }


//- Failure types

type AddUserToGroupError = 
    | ValidationError of string
    | AddError of string
    | DbError of string


//Worflow type 

type AddUserToGroupWorkflow = 
    Group.Group -> User.User -> Result<UserAddedToGroupEvent , AddUserToGroupError>
