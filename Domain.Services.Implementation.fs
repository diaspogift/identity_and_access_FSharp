module IdentityAndAcccess.DomainServices

open System
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAccess.DatabaseTypes
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open MongoDB.Bson



module Group =





    ///Database dependecies
    /// 
    /// 

    let loadOneGroupMemberById = GroupDb.loadOneGroupMemberById
     

     
                



    ///Group type related services
    /// 
    /// 
    type GroupMemberService = {TimeServiceWasCalled:DateTime; CallerCredentials:CallerCredential} with
        member this.isGroupMember : IsGroupMember = 
            fun (getGroupMemberById:GetGroupMemberById) (aGroup:Group) (aMember:GroupMember)  ->
                printfn "I am in the IsGroupMember Function"
                let rec recIsGroupMember  
                    (aMember : GroupMember) 
                    (getGroupMemberById : GetGroupMemberById)
                    (aGroupMemberList : GroupMember list) =

                        match aGroupMemberList with
                        | [] -> 
                            false
                        | head::tail ->

                            if (head.Type = GroupGroupMember) && (head = aMember) then
                                true
                            else 
                            
                                let oneElementListOfAdditionalMembersToCompare (aGroup:Group) : Group list = 
                                    List.init 1 (fun x -> aGroup)
                            
                            ///IO operation here. looking for a group member by its group member identifier - START
                                let additionalGroupToSearch = getGroupMemberById head.MemberId
                            ///IO operation here. - END
                           
                                match additionalGroupToSearch with
                                | Ok anAdditionalGroupToSearch ->

                                    let newMembersToAppend =  anAdditionalGroupToSearch.Members
                                    let allMembers = tail @ newMembersToAppend
                                    let result = recIsGroupMember aMember getGroupMemberById allMembers

                                    result

                                | Error _ -> false


                recIsGroupMember  aMember  getGroupMemberById   aGroup.Members  

                

        member this.isMemberGroupMember :IsGroupMemberWithBakedGetGroupMemberById = 
                this.isGroupMember loadOneGroupMemberById
            

                                

                              
                         



    

     ///Tenant type related services 
     /// 
     /// 
    
             





    ////User type related services 
    /// 
    /// 
    (*let confirmUser aGroup aUser (getUserById:GetUserById)  =      

            //let user = getUserById aUser.UserId

            match 1 with 
            | 1 -> true
            | _ -> false*)



