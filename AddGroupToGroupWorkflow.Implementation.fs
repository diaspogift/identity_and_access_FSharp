namespace IdentityAndAcccess.Workflow.AddGroupToGroupApiTypes

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.Workflow.AddGroupToGroupApiTypes
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









//Step 1 - add group to group 

type AddGroupToGroup = 

    Group.Group -> Group.Group -> Result<Group.Group*GroupMember, AddGroupToGroupError>



//Step 3 - Create events step

type CreateEvents = Group.Group*Group.GroupMember*Group.Group*Group.GroupMember -> GroupAddedToGroupEvent list












module AddGroupToGroupWorkflowImplementation = 



       ///Dependencies
       /// 
       /// 
       let isGroupMemberService  = fun aGroupToAddTo aGroupToAdd -> 

            printfn "===================================="
            printfn "===================================="
            printfn "aGroupToAddTo =                  %A" aGroupToAddTo
            printfn "===================================="
            printfn "===================================="
            printfn "aGroupToAdd =                  %A" aGroupToAdd
            printfn "===================================="
            printfn "===================================="


            let unwrappedGroupToAddTo = aGroupToAddTo |> DomainHelpers.unwrapToStandardGroup
            let unwrappedGroupToAdd = aGroupToAdd |> DomainHelpers.unwrapToStandardGroup

            printfn ""
            printfn ""
            printfn ""
            printfn "===================================="
            printfn "===================================="
            printfn "unwrappedGroupToAddTo =                  %A" unwrappedGroupToAddTo
            printfn "===================================="
            printfn "===================================="
            printfn "unwrappedGroupToAdd =                  %A" unwrappedGroupToAdd
            printfn "===================================="
            printfn "===================================="
            printfn ""
            printfn ""
            printfn ""


            let groupsThatGroupToAddToIsMemberInList = unwrappedGroupToAddTo.MemberIn 
            let groupsThatGroupToAddIsMemberInList = unwrappedGroupToAdd.MemberIn 

            //check that the groupGroupMemberToAdd does not contains the GoupToAddto as a member aka group recursion
            let rsIsGroupTangleRecursion = result {
                let! groupMemberToAdd = (aGroupToAdd |> Group.toMemberOfTypeGroup)
                let rsIsGroupTangleRecursion = groupsThatGroupToAddToIsMemberInList |> List.contains groupMemberToAdd
                printfn "===================================="
                printfn "rsIsGroupTangleRecursion =                  %A" rsIsGroupTangleRecursion
                printfn "===================================="
                printfn ""
                printfn ""
                return rsIsGroupTangleRecursion
                }
            match rsIsGroupTangleRecursion with 
            | Ok rsIsGroupTangleRecursion ->          
                if rsIsGroupTangleRecursion then Error "Group Tangulature !"   
                else 
                    //Check that the groupMember is not already member
                    let rsIsGroupToAddArealdyMember = result {
                            let! groupMemberToAdd = aGroupToAdd |> Group.toMemberOfTypeGroup
                            let rsIsGroupToAddArealdyMember = unwrappedGroupToAddTo.Members |> List.contains groupMemberToAdd
                            return rsIsGroupToAddArealdyMember
                        }
                    match rsIsGroupToAddArealdyMember with 
                    | Ok rsIsGroupToAddArealdyMember ->
                        if rsIsGroupToAddArealdyMember then Error "The group you are trying to add is already a member!" 
                        else
                            //check that the groupToAddTo is not already referenced in the group member we are trying to add
                            let check1 = result {
                                let! groupMemberToAddTo = aGroupToAddTo |> Group.toMemberOfTypeGroup
                                let check1 = groupsThatGroupToAddIsMemberInList |> List.contains groupMemberToAddTo                               
                                return check1 
                                }
                            match check1 with 
                            | Ok aCheck1 ->
                                if aCheck1 then Error "The group you are trying to add is already a member!" 
                                else Ok false
                            | Error error ->
                                Error error  
                    | Error error ->                       
                        Error error
            | Error error ->
                Error error


       //Step 1 add group to group impl
       let add  (groupToAddTo:Group.Group) (groupToAdd:Group.Group)   = 
              
              let addResult = Group.addGroupToGroup groupToAddTo groupToAdd isGroupMemberService
              addResult
              |> Result.mapError AddGroupToGroupError.AddError

              

       //Step2 create events impl
       let createEvents : CreateEvents = 
           fun (group1, memberAddedToG1, group2, memberInAddedToG2 ) ->





(*               type MemberAddedToGroupEvent = { 
                  TenantId : string
                  MemberInAdded : Dto.GroupMember
                  }

              type MemberInAddedToGroupEvent = { 
                  TenantId : string
                  MemberInAdded : Dto.GroupMember
                  }


              type GroupAddedToGroupEvent = { 
                  TenantId : string
                  MemberAdded : MemberAddedToGroupEvent
                  MemberInAdded : MemberInAddedToGroupEvent
                  } *)

              let memberAddedToGroupEvent:MemberAddedToGroupEvent = { 
                  TenantId = memberAddedToG1.TenantId |> TenantId.value
                  MemberAdded = memberAddedToG1 |> GroupMember.fromDomain
                  }

              let memberInAddedToGroupEvent:MemberInAddedToGroupEvent = { 
                  TenantId = memberInAddedToG2.TenantId |> TenantId.value
                  MemberInAdded = memberInAddedToG2 |> GroupMember.fromDomain
                  }


              let groupAddedToGroupEvents:GroupAddedToGroupEvent = {
                     TenantId = memberAddedToG1.TenantId |> TenantId.value
                     MemberAdded = memberAddedToGroupEvent
                     MemberInAdded = memberInAddedToGroupEvent
                     }
        

              let groupAddedToGroupEventList = groupAddedToGroupEvent |> List.singleton
              let groupInAddedToGroupEventList = groupInAddedToGroupEvent |> List.singleton

              let ug1 = group1 |> unwrapToStandardGroup 
              let ug2 = group2 |> unwrapToStandardGroup 
              let ug1id = ug1.GroupId |> GroupId.value 
              let ug2id = ug2.GroupId |> GroupId.value 

              



              printfn "KKKKKKKKKKKKKKKKKKKKKKKJJJJJJJJJJJJJJJJJJJJJJDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD"
              printfn "KKKKKKKKKKKKKKKKKKKKKKKJJJJJJJJJJJJJJJJJJJJJJDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD"
              printfn "KKKKKKKKKKKKKKKKKKKKKKKJJJJJJJJJJJJJJJJJJJJJJDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD"
              printfn "%A = "  (ug1id, groupAddedToGroupEvent, ug2id, groupInAddedToGroupEvent)
              printfn "KKKKKKKKKKKKKKKKKKKKKKKJJJJJJJJJJJJJJJJJJJJJJDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD"
              printfn "KKKKKKKKKKKKKKKKKKKKKKKJJJJJJJJJJJJJJJJJJJJJJDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD"
              printfn "KKKKKKKKKKKKKKKKKKKKKKKJJJJJJJJJJJJJJJJJJJJJJDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD"
              printfn "KKKKKKKKKKKKKKKKKKKKKKKJJJJJJJJJJJJJJJJJJJJJJDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD"

              groupAddedToGroupEventList @  groupInAddedToGroupEventList
              

                         
       ///Add group to group workflow implementation
       /// 
       /// 
       let addGroupToGroupWorkflow: AddGroupToGroupWorkflow = 
           fun groupToAddTo groupToAdd ->
               let createEvents = Result.map createEvents        
               let b = groupToAdd |> add groupToAddTo
               b |> createEvents
               







 
