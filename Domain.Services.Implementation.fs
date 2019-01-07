module IdentityAndAcccess.DomainServices

open System
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open MongoDB.Bson
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes
open Suave.Sockets
open IdentityAndAcccess.DomainTypes







let unwrapToStandardGroup aGroupToUnwrapp = 
        match aGroupToUnwrapp with 
            | Standard aStandardGroup -> aStandardGroup
            | Internal anInternalGroup -> anInternalGroup




module Group =





    ///Database dependecies 
     
    
    let loadGroupByIdDbDependencyFunction = GroupDb.loadOneGroupById


    let loadGroupByGroupMemberIdDbDependencyFunction = GroupDb.loadOneGroupMemberById
     

     
                



    ///services
    
    
    let isGroupMemberIsInGroupServiceImpl : IsGroupMemberService =

        fun loadGroupMemberById  // Database dependency function
            aGroup 
            aMember -> 
                
            let aStandardGroup = aGroup 
                                 |> DomainHelpers.unwrapToStandardGroup


                    
            let rec InternalRecursiveIsGroupMemberIsInGroupService  
                                                                    (aMember : GroupMember)  
                                                                    (loadGroupMemberById : LoadGroupMemberById) 
                                                                    (aGroupMemberList : GroupMember list) =

                    match aGroupMemberList with
                    | [] -> 
                        false
                    | head::tail ->

                        if (head.Type = GroupGroupMember) && (head = aMember) then
                            true
                        else 
                        
                        ///IO operation here. looking for a group member by its group member identifier - START
                            let additionalGroupToSearch = loadGroupMemberById head.MemberId
                        ///IO operation here. - END
                       
                            match additionalGroupToSearch with
                            | Ok anAdditionalGroupToSearch ->

                                let aStandardAdditionalGroupToSearch = anAdditionalGroupToSearch 
                                                                       |> DomainHelpers.unwrapToStandardGroup
                                                                       
                                let newMembersToAppend =  aStandardAdditionalGroupToSearch.Members
                                let allMembers = tail @ newMembersToAppend
                                let result = InternalRecursiveIsGroupMemberIsInGroupService aMember loadGroupMemberById allMembers

                                result

                            | Error _ -> false


            InternalRecursiveIsGroupMemberIsInGroupService  aMember  loadGroupMemberById   aStandardGroup.Members








    let isUserInNestedGroupServiceImpl : IsUserInNestedGroupService = 

        fun  
            loadGroupMemberById  //Database dependency
            aGroup 
            aUser ->




            let rec InternalRecursiveIsUserInNestedGroupService  
                                                                    (aMember : GroupMember)  
                                                                    (loadGroupMemberById : LoadGroupMemberById) 
                                                                    (aGroupMemberList : GroupMember list) =

                    match aGroupMemberList with
                    | [] -> 
                        false
                    | head::tail ->

                        if (head.Type = GroupGroupMember) && (head = aMember) then
                            true
                        else 
                        
                        
                        ///IO operation here. looking for a group member by its group member identifier - START
                            let additionalGroupToSearch = loadGroupMemberById head.MemberId
                        ///IO operation here. - END
                       
                            match additionalGroupToSearch with
                            | Ok anAdditionalGroupToSearch ->

                                let aStandardAdditionalGroupToSearch = anAdditionalGroupToSearch 
                                                                       |> DomainHelpers.unwrapToStandardGroup
                                                                       
                                let newMembersToAppend =  aStandardAdditionalGroupToSearch.Members
                                let allMembers = tail @ newMembersToAppend
                                let result = InternalRecursiveIsUserInNestedGroupService aMember loadGroupMemberById allMembers

                                result

                            | Error _ -> false


            

            


            let aStandardGroup = aGroup 
                                 |> DomainHelpers.unwrapToStandardGroup
            let aUserGroupMember = aUser |> User.toUserGroupMember

            match aUserGroupMember with  
            | Ok aMember ->

                InternalRecursiveIsUserInNestedGroupService  aMember  loadGroupMemberById   aStandardGroup.Members
                   
            | Error error ->
                false

        
        


    let groupMemberServices: GroupMemberServices = {
   
        TimeServiceWasCalled = DateTime.Now
        CallerCredentials = CallerCredential "FOTIO"
        isGroupMember = isGroupMemberIsInGroupServiceImpl
        isUserInNestedGroup = isUserInNestedGroupServiceImpl

    }

    

    
          

                


                                

///User type related services 
/// 
///                        
module User =



    ///Database dependecies 
     
    
    let loadUserByUserIdAndTenantIdDependencyFunction = UserDb.loadUserByUserIdAndTenantId


     
    










    let confirmUserServiveImpl : ConfirmUserServive = 

        fun loadUserByUserIdAndTenantId aGroup aUser ->

            let rsConfirmUser = result {


                let unWrappedGroup  =  aGroup |> unwrapToStandardGroup 
                let tenantIdFromUnWrappedGroup = unWrappedGroup.TenantId
                let userId = aUser.UserId

                let! confirmedUser = loadUserByUserIdAndTenantId userId tenantIdFromUnWrappedGroup


                     
                return confirmedUser
            }
            

            match rsConfirmUser with 
            | Ok confirmUser ->
                true
            | Error error ->
                false







