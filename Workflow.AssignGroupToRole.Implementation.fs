namespace IdentityAndAcccess.Workflow.AssignGroupToRoleApiTypes

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
open System.Text.RegularExpressions
open IdentityAndAcccess.DomainTypes.Functions.Dto
open FSharp.Data.Sql
open IdentityAndAcccess.DomainTypes.Functions.Dto
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.Functions.Dto
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.Functions.Dto
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.DomainTypes.User






///Assign group to role worflow types
/// 
/// 


//Assign group to role workflow input types
type UnvalidatedRoleAndGroupIds = {
        RoleId : string
        GroupId: string
    }


///Ouputs of the Assign group to role worflow 
//- Sucess types





type GroupAssignedToRoleEvent = { 
    RoleId : string
    GroupId : string
    AssignedGroup : Dto.GroupDescriptor
    }


//- Failure types

type AssignGroupToRoleError = 
    | ValidationError of string
    | AssignError of string
    | DbError of string


type AssignGroupToRoleWorkflow = 
    Role.Role -> Group.Group -> Result<GroupAssignedToRoleEvent , AssignGroupToRoleError>


//Step 1 - add group to group 

type AssignGroupToRole= 

    Role.Role -> Group.Group -> Result<Role.Role*User.GroupDescriptor, AssignGroupToRoleError>



//Step 3 - Create events step

type CreateEvents = Role.Role*User.GroupDescriptor -> GroupAssignedToRoleEvent












module AssignGroupToRoleWorkflowImplementation = 



       ///Dependencies
       /// 
       /// 


       //Step 1 assign group to role impl
       let add  (roleToAssignTo:Role.Role) (groupToAssignToRole:Group.Group)   = 
              
              let assignResult = Role.assignGroup roleToAssignTo groupToAssignToRole 
              assignResult
              |> Result.mapError AssignGroupToRoleError.AssignError

              

       //Step2 create events impl
       let createEvents : CreateEvents = 
           fun (aRole, aGroupDesc) ->
           let groupAssignedToRoleEvent : GroupAssignedToRoleEvent = {
               RoleId = aRole.RoleId |> RoleId.value
               GroupId = aGroupDesc.Id |> GroupId.value
               AssignedGroup = aGroupDesc |> GroupDescriptor.fromDomain
           }
           groupAssignedToRoleEvent
              

                         
       ///assign group to role workflow implementation
       /// 
       /// 
       let assignGroupToRoleWorkflow: AssignGroupToRoleWorkflow = 
           fun roleToAssignTo groupToAdd ->
               let createEvents = Result.map createEvents        
               let b = groupToAdd |> add roleToAssignTo
               b |> createEvents
               







 
