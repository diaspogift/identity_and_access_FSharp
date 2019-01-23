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



















//Step 1 - add group to group 

type AddGroupToGroup = 

    Group -> Group -> Result<Group*GroupMember, AddGroupToGroupError>



//Step 3 - Create events step

type CreateEvents = Group * GroupMember -> GroupAddedToGroupEvent









///Step 1 validate group info to add to impl






///Step1 adds user to group impl

//Dependencies 

let isGroupMemberService = Group.isGroupMemberIsInGroupServiceImpl

let add : AddGroupToGroup = 
    fun groupToAddTo groupToAdd ->
       Group.addGroupToGroup groupToAddTo groupToAdd isGroupMemberService
       |> Result.mapError AddGroupToGroupError.AddError

       

///Step2 create events impl
let createEvents : CreateEvents = 
    fun (aGroup, aGroupMember) ->
       let groupAddedToGroupEvent : GroupAddedToGroupEvent = {
           GroupAddedTo = (aGroup |> DbHelpers.fromGroupDomainToDto)
           GroupMemberAdded =  (aGroupMember |> DbHelpers.fromGroupMemberToGroupMemberDto)         
       }
       groupAddedToGroupEvent

        
                    

 

///Add group to group workflow implementation
/// 
/// 
let addGroupToGroupWorkflow: AddGroupToGroupWorkflow = 
    fun groupToAddTo groupToAdd ->
        let createEvents = Result.map createEvents
        groupToAddTo
        |> add groupToAdd 
        |> createEvents
        







 
