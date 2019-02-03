namespace IdentityAndAcccess.Workflow.AddUserToGroupApiTypes

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainTypes.Functions.Dto
open IdentityAndAcccess.DomainTypes.User



///Add user to group worflow types
/// 
/// 


//add user to group workflow input types
type UnvalidatedGroupAndUserId = {
        GroupId : string
        UserId: string
    }





///Ouputs of the add user to group worflow 
//- Sucess types




type UserAddedToGroupEvent = { 
    GroupId : string
    TenantId : string
    AddedUser : Dto.UserDescriptor
    }


//- Failure types

type AddUserToGroupError = 
    | ValidationError of string
    | AddError of string
    | DbError of string


//Worflow type 

type AddUserToGroupWorkflow = 
    Group.Group -> User.User -> Result<UserAddedToGroupEvent , AddUserToGroupError>
















//Step 1 - add user to group 

type AddUserToGroup = 

    Group.Group -> User.User -> Result<Group.Group*User.UserDescriptor, AddUserToGroupError>



//Step 3 - Create events step

type CreateEvents = Group.Group * User.UserDescriptor -> UserAddedToGroupEvent









///Step 1 validate group info to add to impl



module AddUserToGroupWorfklowImplementation = 


    ///Step1 adds user to group impl
    let add : AddUserToGroup = 
        fun groupToAddTo user ->
           Group.addUserToGroup groupToAddTo user
           |> Result.mapError AddUserToGroupError.AddError

           

    ///Step2 create events impl
    let createEvents : CreateEvents = 
        fun (aGroup, aUser) ->
           let userAddedToGroupEvent : UserAddedToGroupEvent = {
               GroupId = aGroup.GroupId |> GroupId.value
               TenantId = aGroup.TenantId |> TenantId.value
               AddedUser =  aUser |> UserDescriptor.fromDomain  
           }
           userAddedToGroupEvent

            
                        

     

    ///Add user to group workflow implementation
    /// 
    /// 
    let addUserToGroupWorkflow: AddUserToGroupWorkflow = 
        fun group user ->
            let createEvents = Result.map createEvents
            user
            |> add group 
            |> createEvents
            







     
