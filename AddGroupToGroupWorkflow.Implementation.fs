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



















//Step 1 - add group to group 

type AddGroupToGroup = 

    Group.Group -> Group.Group -> Result<Group.Group*GroupMember, AddGroupToGroupError>



//Step 3 - Create events step

type CreateEvents = Group.Group * Group.GroupMember -> GroupAddedToGroupEvent









///Step 1 validate group info to add to impl






///Step1 adds user to group impl

//Dependencies 
let isGroupMemberService = Group.isGroupMemberIsInGroupServiceGreyYoungEventStoreImpl



let add  (groupToAddTo:Group.Group) (groupToAdd:Group.Group)   = 
       
       let addResult = Group.addGroupToGroup groupToAddTo groupToAdd isGroupMemberService
       addResult
       |> Result.mapError AddGroupToGroupError.AddError

       

///Step2 create events impl
let createEvents : CreateEvents = 
    fun (aGroup, aGroupMember) ->
       let groupAddedToGroupEvent : GroupAddedToGroupEvent = {
           GroupMemberAdded =  aGroupMember |> Dto.GroupMember.fromDomain   
       }
       groupAddedToGroupEvent

                  
///Add group to group workflow implementation
/// 
/// 
let addGroupToGroupWorkflow: AddGroupToGroupWorkflow = 
    fun groupToAddTo groupToAdd ->
        let createEvents = Result.map createEvents        
        groupToAdd
        |> add groupToAddTo
        |> createEvents
        







 
