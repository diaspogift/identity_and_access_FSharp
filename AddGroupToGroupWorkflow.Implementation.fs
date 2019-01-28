module IdentityAndAcccess.Workflow.AddGroupToGroupApiTypes.AddGroupToGroupApiTypes

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainServicesImplementations.Tenant
open IdentityAndAcccess.Workflow.AddGroupToGroupApiTypes
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainServicesImplementations
open IdentityAndAcccess.DomainTypes
open IdentityAndAccess.DatabaseTypes

open IdentityAndAcccess.DomainServicesImplementations
open IdentityAndAcccess.DomainTypes.Functions.Dto
open System.Text.RegularExpressions
open IdentityAndAcccess.EventStorePlayGround.Implementation
open IdentityAndAcccess.DomainTypes.Functions.Dto
open FSharp.Data.Sql
open IdentityAndAcccess.DomainTypes.Functions.Dto



















//Step 1 - add group to group 

type AddGroupToGroup = 

    Group.Group -> Group.Group -> Result<Group.Group*GroupMember, AddGroupToGroupError>



//Step 3 - Create events step

type CreateEvents = Group.Group*Group.GroupMember*Group.Group*Group.GroupMember -> string*GroupStreamEvent list* string*GroupStreamEvent list









///Step 1 validate group info to add to impl






///Step1 adds user to group impl

//Dependencies 
let isGroupMemberService = Group.isGroupMemberIsInGroupServiceLocalImpl



let add  (groupToAddTo:Group.Group) (groupToAdd:Group.Group)   = 
       
       let addResult = Group.addGroupToGroup groupToAddTo groupToAdd isGroupMemberService
       addResult
       |> Result.mapError AddGroupToGroupError.AddError

       

///Step2 create events impl
let createEvents : CreateEvents = 
    fun (group1, memberAddedToG1, group2, memberInAddedToG2 ) ->

       let ug1 = group1 |> unwrapToStandardGroup 
       let ug2 = group2 |> unwrapToStandardGroup 
       let ug1id = ug1.GroupId |> GroupId.value 
       let ug2id = ug2.GroupId |> GroupId.value 

       let groupAddedToGroupEventData = memberAddedToG1 |> GroupMember.fromDomain
       let groupAddedToGroupEvent = groupAddedToGroupEventData |> GroupStreamEvent.GroupAddedToGroup |> List.singleton

       let groupInAddedToGroupEventData = memberInAddedToG2 |> GroupMember.fromDomain 
       let groupInAddedToGroupEvent = groupInAddedToGroupEventData |> GroupStreamEvent.GroupInAddedToGroup |> List.singleton


       printfn "KKKKKKKKKKKKKKKKKKKKKKKJJJJJJJJJJJJJJJJJJJJJJDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD"
       printfn "KKKKKKKKKKKKKKKKKKKKKKKJJJJJJJJJJJJJJJJJJJJJJDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD"
       printfn "KKKKKKKKKKKKKKKKKKKKKKKJJJJJJJJJJJJJJJJJJJJJJDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD"
       printfn "%A = "  (ug1id, groupAddedToGroupEvent, ug2id, groupInAddedToGroupEvent)
       printfn "KKKKKKKKKKKKKKKKKKKKKKKJJJJJJJJJJJJJJJJJJJJJJDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD"
       printfn "KKKKKKKKKKKKKKKKKKKKKKKJJJJJJJJJJJJJJJJJJJJJJDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD"
       printfn "KKKKKKKKKKKKKKKKKKKKKKKJJJJJJJJJJJJJJJJJJJJJJDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD"
       printfn "KKKKKKKKKKKKKKKKKKKKKKKJJJJJJJJJJJJJJJJJJJJJJDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD"

       
       (ug1id, groupAddedToGroupEvent, ug2id, groupInAddedToGroupEvent)

                  
///Add group to group workflow implementation
/// 
/// 
let addGroupToGroupWorkflow: AddGroupToGroupWorkflow = 
    fun groupToAddTo groupToAdd ->
        let createEvents = Result.map createEvents        
        let b = groupToAdd |> add groupToAddTo
        b |> createEvents
        







 
