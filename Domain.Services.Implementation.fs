module IdentityAndAcccess.DomainServicesImplementations

open System
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces

open IdentityAndAcccess.EventStorePlayGround.Implementation
open IdentityAndAcccess.DomainTypes.Functions

open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Role
open FSharp.Data.Sql
open System.Text.RegularExpressions








let unwrapToStandardGroup aGroupToUnwrapp = 
    match aGroupToUnwrapp with 
    | Standard aStandardGroup -> aStandardGroup
    | Internal anInternalGroup -> anInternalGroup

























///Group type related services 
/// 
///  
module Group =





    ///Mongo Database dependecies 
    
    //let loadGroupByIdMongoDependencyFunction = GroupDb.loadOneGroupById

    //let loadGroupByGroupMemberIdDbDependencyFunction = GroupDb.loadOneGroupMemberById



    ///Gey Young Event Store Database dependecies 
    
    let loadGroupByIdGreyYoungEventStoreDependencyFunction = EventStorePlayGround.loadGroupWithGroupMemberId
    
    let loadGroupByGroupMemberIdGreyYoungEventStoreDependencyFunction = EventStorePlayGround.loadGroupWithGroupMemberId
     
                



    ///services
    
    
    let isGroupMemberIsInGroupServiceLocalImpl  = fun aGroupToAddTo aGroupToAdd -> 

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

           


                    
            






    let isUserInNestedGroupService : IsUserInNestedGroupService = 

        fun   aGroup aUserMember ->

            let rec recursiveCheck (aUserMemberToChechIfInNestedGroup : GroupMember) (aGroupMemberList : GroupMember list) =
                                                                
                match aGroupMemberList with
                | [] -> 
                    let rs0 = ([], false)
                    rs0
                | firstGroupMember::remainingGroupMembers ->

                    if (firstGroupMember.Type = GroupMemberType.UserGroupMember) && (aUserMemberToChechIfInNestedGroup = firstGroupMember)    then
                        let rs1 = ([], true)
                        rs1
                    else if (firstGroupMember.Type = GroupMemberType.GroupGroupMember) && (aUserMemberToChechIfInNestedGroup <> firstGroupMember) then
                        let rsSearch,
                            _ = recursiveCheck aUserMemberToChechIfInNestedGroup remainingGroupMembers
                        let rs2 = (rsSearch@[firstGroupMember], false)
                        rs2
                    else 
                        let rs3 = ([], false)
                        rs3

                    
                        
            let uwGroup = aGroup |> unwrapToStandardGroup 
            let uwGroupMembers = uwGroup.Members
                
            recursiveCheck aUserMember uwGroupMembers
                   

            
         

        
    
    

    
          

                









module Role =


    ()


    ///Database dependecies 
     
    


     
    







    //Authorisation relatedservices

    // let isUserInRoleServiceImpl : IsUserInRoleService = 

    //     fun  isUserInNestedGroupService confirmUserServive aRole aUser -> 

    //         match aUser.Enablement.EnablementStatus with 
    //         | Enabled -> 

    //             Role.isInRole isUserInNestedGroupService confirmUserServive aRole aUser

    //         | Disabled ->

    //             false


        
















                                

///User type related services 
/// 
///                        
module User =



    ///Database dependecies 
     
    


     
    










    //Domain related services
    let confirmUserServiveImpl : ConfirmUserServive = 

        fun aGroup aUser ->
            let unWrappedGroup  =  aGroup |> unwrapToStandardGroup 
            aUser.TenantId = unWrappedGroup.TenantId

            



    let passwordMatcherService : PasswordsMatcherService = 
        fun aPassword anEncryptedPasssword ->
            true





    // let allRolesForIdentifiedUser (isUserInNestedGroup:IsUserInNestedGroupService')  //Dependency
    //                               (confirmUserServive:ConfirmUserServive')          //Dependency
    //                               (allTenantRoles:Role list) (aTenant:Tenant) (aUser:User) : RoleDescriptor list = 

    //     let fromRoleToRoleDescriptor (aRole:Role) =
    //          {RoleId = aRole.RoleId; TenantId = aRole.TenantId; Name = aRole.Name}



    //     let isUserPlayingRole aUser aRole  = 
    //         Role.isInRole isUserInNestedGroup confirmUserServive aRole aUser

    //     let isUserPlayingRole' = isUserPlayingRole  aUser

    //     allTenantRoles
    //     |> List.filter isUserPlayingRole'
    //     |> List.map fromRoleToRoleDescriptor




    let authenticate : AuthenticationService = 
        fun aPasswordMatcherService             // Dependency
            isUserInNestedGroupService          // Dependency
            aUser aTenant aPassword ->

            match aTenant.ActivationStatus with
            | ActivationStatus.Activated ->

                match aUser.Enablement.EnablementStatus with
                | EnablementStatus.Enabled ->

                    let rsPasswordComparison = result {
                        let strUserEncryptedPassWord  = aUser.Password |> Password.value 
                        let! userEncryptedPassWord = strUserEncryptedPassWord |> EncrytedPassword.create'
                        let areSamePasswords = aPasswordMatcherService aPassword userEncryptedPassWord
                        return areSamePasswords
                    }

                    match rsPasswordComparison with 
                    | Ok areSamePasswords ->

                        if areSamePasswords then 
                            let userDescriptor = aUser |> User.toUserDesriptor
                            userDescriptor
                        else
                            Error "Password does not match"

                    | Error error ->
                        Error error

                    

                

                | EnablementStatus.Disabled ->


                    let msg = "User enablement status is desabled"

                    Error msg

            | Deactivated ->

                let msg = "User tenant Activation status is deactivated"

                Error msg





    let authenticate' = authenticate passwordMatcherService
