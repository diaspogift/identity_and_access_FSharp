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
    let loadGroupByGroupMemberIdDbDependencyFunctionAapted = GroupDb.loadOneGroupMemberById
     

     
                



    ///Group type related services
    /// 
    ///
    let isGroupMember (getGroupMemberById:LoadGroupMemberById) (aGroup:Group) (aMember:GroupMember) : bool =
                
                
        let rec recIsGroupMember  (aMember : GroupMember) (getGroupMemberById : LoadGroupMemberById) (aGroupMemberList : GroupMember list) =

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

        
        
        
        
    let groupMemberService: GroupMemberServices = {
   
        TimeServiceWasCalled = DateTime.Now
        CallerCredentials = CallerCredential "FOTIO"
        isGroupMember = isGroupMember

    }

    

    
          

                


                                

                        

     ///Tenant type related services 
     /// 
     /// 
    
             







