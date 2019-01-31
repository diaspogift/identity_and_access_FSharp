namespace IdentityAndAcccess.Workflow.AssignUserToRoleApiTypes

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
open IdentityAndAcccess.DomainTypes.Group



///Assign user to role worflow types
/// 
/// 


//Assign user to role workflow input types
type UnvalidatedRoleAndUserId = {
        RoleId : string
        UserId: string
    }


///Ouputs of the assign user to role worflow 
//- Sucess types




type UserAssignedToRoleEvent = { 
    RoleId : string
    UserId : string
    UserAssigned : Dto.GroupMember
    }


//- Failure types

type AssignUserToRoleError = 
    | ValidationError of string
    | DbError of string
    | AssignError of string


//Worflow type 

type AssignUserToRoleWorkflow = 
    Role.Role -> User.User -> Result<UserAssignedToRoleEvent , AssignUserToRoleError>
















//Step 1 - assign user to role 

type AssignUserToRole = 

    Role.Role -> User.User -> Result<Role.Role*Group.GroupMember, AssignUserToRoleError>



//Step 3 - Create events step

type CreateEvents = Role.Role * Group.GroupMember -> UserAssignedToRoleEvent









///Step 1 validate user info to add to impl



module AssignUserToRoleWorfklowImplementation = 


    ///Step1 assign user to role impl
    let assign : AssignUserToRole = 
        fun roleToAssignTo user ->
             Role.assignUser roleToAssignTo user
             |> Result.mapError AssignUserToRoleError.AssignError

           

    ///Step2 create events impl
    let createEvents : CreateEvents = 
        fun (aRole, aGroupMember) ->
           let userAssignedToRoleEvent : UserAssignedToRoleEvent = {
               RoleId = aRole.RoleId |> RoleId.value
               UserId = aGroupMember.MemberId |> GroupMemberId.value
               UserAssigned = aGroupMember |> GroupMember.fromDomain
           }
           userAssignedToRoleEvent

            
                        

     

    ///Assign user to role workflow implementation
    /// 
    /// 
    let assignUserToRoleWorkflow (role:Role.Role) (user:User.User) : Result<UserAssignedToRoleEvent, AssignUserToRoleError>  = 
          
            let createEvents = Result.map createEvents
            user
            |> assign role 
            |> createEvents
            







     
