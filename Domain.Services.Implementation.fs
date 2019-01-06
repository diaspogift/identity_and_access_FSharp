module IdentityAndAcccess.DomainServices

open System
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAccess.DatabaseTypes
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.RoleDb
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open MongoDB.Bson




module Group =





    ///Database dependecies
    /// 
    /// 
    let loadGroupByIdDbDependencyFunction = GroupDb.loadOneGroupById
    let loadGroupByGroupMemberIdDbDependencyFunction = GroupDb.loadOneGroupMemberById
     

     
                



    ///Group type related domain services
    /// 
    ///
    let isGroupMember (loadGroupMemberById:LoadGroupMemberById) (aGroup:Group) (aMember:GroupMember) : bool =
                
                
        let rec recIsGroupMember  (aMember : GroupMember) (loadGroupMemberById : LoadGroupMemberById) (aGroupMemberList : GroupMember list) =

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
                    let additionalGroupToSearch = loadGroupMemberById head.MemberId
                ///IO operation here. - END
               
                    match additionalGroupToSearch with
                    | Ok anAdditionalGroupToSearch ->

                        let newMembersToAppend =  anAdditionalGroupToSearch.Members
                        let allMembers = tail @ newMembersToAppend
                        let result = recIsGroupMember aMember loadGroupMemberById allMembers

                        result

                    | Error _ -> false


        recIsGroupMember  aMember  loadGroupMemberById   aGroup.Members

        
    let isGroupMember' = isGroupMember loadGroupByGroupMemberIdDbDependencyFunction
        
        
    let groupMemberService: GroupMemberServices = {
   
        TimeServiceWasCalled = DateTime.Now
        CallerCredentials = CallerCredential "FOTIO"
        isGroupMember = isGroupMember

    }

    

    
          

                


                                

                        

     ///Tenant type related services 
     /// 
     /// 
    
             







