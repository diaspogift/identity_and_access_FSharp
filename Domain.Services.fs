module IdentityAndAcccess.DomainServices


open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.CommonDomainTypes
open System
open IdentityAndAccess.DatabaseTypes
open IdentityAndAccess.DatabaseTypes.Functions
open MongoDB.Bson



module Group =


    ///Types
    type GetUserById = UserId -> Result<User, string>
    type GetGroupById = GroupId -> Result<Group, string>
    type GetGroupMemberById = GroupMemberId -> Result<Group, string>
    type IsGroupMember = GetGroupMemberById -> Group -> GroupMember -> Boolean
    type IsGroupMemberWithBakedGetGroupMemberById =  Group -> GroupMember -> Boolean
    type IsUserInNestedGroup = Group -> User -> GetGroupMemberById -> Boolean
    type CallerCredential = CallerCredential of string

    ///Database dependecies
    let LoadGroupByIdDbDependencyFunction = GroupDb.loadOneGroupById

    let loadGroupByIdDbDependencyAdapted 
        (f:BsonObjectId -> GroupDto) 
        : (GroupMemberId -> Result<Group, string>)  =

        let fToreturn 
                (aGroupMemberId:GroupMemberId) 
                :Result<Group, string> =

                let groupResult = result {

                    let bisonId = new BsonObjectId(new ObjectId((GroupMemberId.value aGroupMemberId)))
                    let groupDto = bisonId |> GroupDb.loadOneGroupById
                    let! group = groupDto |> DbHelpers.fromDbDtoToGroup 

                    return group
                }
                
                match groupResult with  
                | Ok groupResult -> Ok groupResult
                | Error error -> Error error
            
        fToreturn

    let LoadGroupByIdDbDependencyFunctionAapted = loadGroupByIdDbDependencyAdapted LoadGroupByIdDbDependencyFunction
        
                
    //let IsGroupMemberWithBakedGetGroupMemberById:IsGroupMemberWithBakedGetGroupMemberById =  loadGroupByIdDbDependencyAdapted loadGroupByIdDbDependency . 



    //Group type related services
    type GroupMemberService = {TimeServiceWasCalled:DateTime; CallerCredentials:CallerCredential} with
        member this.IsGroupMember :IsGroupMember = 
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
                            printfn "THE HEAD IS: %A" head
                            if (head.Type = GroupGroupMember) && (head = aMember) then
                                true
                            else 
                                let oneElementListOfAdditionalMembersToCompare (aGroup:Group) : Group list = 
                                    List.init 1 (fun x -> aGroup)
                            ///IO operation here. looking for a group member by its group member identifier 
                                let additionalGroupToSearch = getGroupMemberById head.MemberId

                                

                                //init : int -> (int -> 'T) -> 'T list
                            ///IO operation here.
                                match additionalGroupToSearch with
                                | Ok anAdditionalGroupToSearch ->

                                    let newMembersToAppend =  anAdditionalGroupToSearch.Members
                                    //let newMembertoToAppendList = oneElementListOfAdditionalMembersToCompare newMembersToAppend
                                    let allMembers = tail @ newMembersToAppend
                                    let result = recIsGroupMember aMember getGroupMemberById allMembers

                                    result

                                | Error _ -> false

                         


                recIsGroupMember  aMember  getGroupMemberById   aGroup.Members  

                

        member this.IsMemberGroupMember :IsGroupMemberWithBakedGetGroupMemberById = this.IsGroupMember LoadGroupByIdDbDependencyFunctionAapted
            

                                

                              
                         



    

     //Tenant type related services 
    
             

    //User type related services 
    let confirmUser aGroup aUser (getUserById:GetUserById)  =      

            //let user = getUserById aUser.UserId

            match 1 with 
            | 1 -> true
            | _ -> false



