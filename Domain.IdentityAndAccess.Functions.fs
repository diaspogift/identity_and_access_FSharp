module IdentityAndAcccess.DomainTypes.Functions

open System
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainTypes
open FSharp.Data.Sql








let generateNoEscapeId () = Guid.NewGuid().ToString().Replace("-", "")


type IsGroupMember' =  Group -> GroupMember  -> Boolean


//Services that act on domain types

module ServiceInterfaces =




     
    


    //Others

    type CallerCredential = CallerCredential of string
        
    //Database dependencies interfaces to be used by the domain services
    //TO DO Should they be here ????????????

    type GetGroupById = GroupId -> Result<Group, string>
    type LoadGroupMemberById = GroupMemberId -> Result<Group, string>

    //Domaim services dependencies interfaces for domain business logic to be used by the
    //group member services
    
    type IsGroupMember = LoadGroupMemberById -> Group -> GroupMember   -> Boolean
    type IsGroupMemberWithBakedGetGroupMemberById = Group -> GroupMember   -> Boolean
    type IsUserInNestedGroup = Group -> User -> LoadGroupMemberById -> Boolean
    
    



    ///Databases interface dependencies for domain : inward dependecies must be followes 
    /// 
    /// 
    type GroupDatabaseServices = {

        getGroupById: GetGroupById
        getGroupMemberById: LoadGroupMemberById
    }
    

    ///Domain services interfaces : Necessary for inward dependencies - implemetation in anoter layer
    /// 
    ///
    type GroupMemberServices = {

        //Service caractheristics
        TimeServiceWasCalled: DateTime
        CallerCredentials: CallerCredential
        //Service funtions
        isGroupMember: IsGroupMember
        //isGroupMemberWithBakedGetGroupMemberById : IsGroupMemberWithBakedGetGroupMemberById
        //isUserInNestedGroup: IsUserInNestedGroup

    }

  



    

    
    
             

 









///Function that act on Domain types


module RegistrationInvitations =

    let fromRegistrationInvitationDtoTempToDomain = 
        fun aInvitationDto ->                                     
            result {
                                
                let! invitationId = aInvitationDto.RegistrationInvitationId |> RegistrationInvitationId.create "invitation id: " 
                let! tenantId = aInvitationDto.TenantId |> TenantId.create "tenantId" 
                let! registrationInvitationDescription = aInvitationDto.Description |> RegistrationInvitationDescription.create "tenant description" 
                let startingOn =  aInvitationDto.StartingOn
                let until = aInvitationDto.Until

                let registrationInvitation:RegistrationInvitation = {
                        RegistrationInvitationId = invitationId
                        TenantId = tenantId
                        Description = registrationInvitationDescription
                        StartingOn = startingOn
                        Until = until
                        }  

                return registrationInvitation
            }





    let create fieldName (invitationsDto:RegistrationInvitationDtoTemp list) =  
        invitationsDto
        |> List.map fromRegistrationInvitationDtoTempToDomain




    let isIdentifiedById (aRegistrationInvitationId : RegistrationInvitationId)(aRegistrationInvitation : RegistrationInvitation) : Boolean = 
        match (aRegistrationInvitation.RegistrationInvitationId = aRegistrationInvitationId) with 
        | true -> true
        | false -> false



    let invitation  (aRegistrationInvitationId : RegistrationInvitationId) (aRegistrationInvitationList : RegistrationInvitation list) =
        aRegistrationInvitationList 
        |> List.filter (fun nextRegistrationInvitation -> isIdentifiedById aRegistrationInvitationId nextRegistrationInvitation)
        |> List.tryHead





    let isAvailable (aTimeNow : DateTime) (aRegistrationInvitation : RegistrationInvitation) : Boolean =

        let resultCompareTimeNowAndSartDate = DateTime.Compare (aTimeNow, aRegistrationInvitation.StartingOn)
        let resultCompareTimeNowAndEndDate = DateTime.Compare (aTimeNow, aRegistrationInvitation.Until)
        let resultCombinedOfResultSartDateAndEndDate = (resultCompareTimeNowAndSartDate < 0 || resultCompareTimeNowAndEndDate < 0)

        match resultCombinedOfResultSartDateAndEndDate  with  
        | true ->  false
        | false -> true





    let isAvailableWithBakedDateTimeParam = isAvailable DateTime.Now
       



    let isNotAvailable (aTimeNow : DateTime) (aRegistrationInvitation : RegistrationInvitation): Boolean =

        let resultCompareTimeNowAndSartDate = DateTime.Compare (aTimeNow, aRegistrationInvitation.StartingOn)
        let resultCompareTimeNowAndEndDate = DateTime.Compare (aTimeNow, aRegistrationInvitation.Until)
        let resultCombinedOfResultSartDateAndEndDate = (resultCompareTimeNowAndSartDate < 0 || resultCompareTimeNowAndEndDate < 0)
        
        match  resultCombinedOfResultSartDateAndEndDate with  
        | true ->  true
        | false -> false
          




    let isNotAvailableWithBakedDateTimeParam = isNotAvailable DateTime.Now
















module Tenant = 






    let create id name description = 
        
        result {

            let! ids = TenantId.create "tenant id: " id
            let! names = TenantName.create "tenant name: " name
            let! descriptions = TenantDescription.create "tenant description: " description
            
            return {
                    TenantId = ids
                    Name = names
                    Description = descriptions
                    RegistrationInvitations = []
                    ActivationStatus = ActivationStatus.Activated
            }
        }






    let fullCreate id name description activationStatus = 
        
        result {

            let! ids = TenantId.create "tenant id: " id
            let! names = TenantName.create "tenant name: " name
            let! descriptions = TenantDescription.create "tenant description: " description
            
            return {
                    TenantId = ids
                    Name = names
                    Description = descriptions
                    RegistrationInvitations = []
                    ActivationStatus = activationStatus
            }
        }






    let isRegistrationInvitationAvailableThrough (aTenant : Tenant) (aRegistrationInvitationId : RegistrationInvitationId) : Boolean =
        
        match aTenant.ActivationStatus with 
        | Activated -> 
            let result = RegistrationInvitations.invitation aRegistrationInvitationId aTenant.RegistrationInvitations
            match result with  
            | Some _ -> true
            | None -> false
        | Disactivated 
            -> false
        




    let offerRegistrationInvitation (aTenant : Tenant) (aDescription : RegistrationInvitationDescription) : Result<Tenant, string> =
        
          
        if aTenant.ActivationStatus = ActivationStatus.Activated then 
            
            result {

                    let id = Guid.NewGuid().ToString().Replace("-", "")
                    let! registrationInvitationId = RegistrationInvitationId.create "registration invitation id" id

                    let registrationInvitation : RegistrationInvitation = {
                            Description = aDescription
                            RegistrationInvitationId = registrationInvitationId
                            StartingOn = DateTime.Now
                            TenantId = aTenant.TenantId
                            Until = DateTime.Now
                            }

                    let tenantWithNewRegistrationInvitation = { aTenant with RegistrationInvitations = [registrationInvitation]@aTenant.RegistrationInvitations}

                    return tenantWithNewRegistrationInvitation 
                }
        else 
            let msg =  "Could not offer registration invitation" 
            Error  msg





    let provisionGroup (aTenant : Tenant)(aGroupName :GroupName) (aGroupDescription: GroupDescription) : Result<Group, string> =

        let id = generateNoEscapeId();
        let tenantId = (TenantId.value aTenant.TenantId)

        if aTenant.ActivationStatus = ActivationStatus.Activated then

            let group = result {

                let! groupId = GroupId.create "group id" id

                let group = {
                    GroupId = groupId 
                    TenantId = aTenant.TenantId
                    Name = aGroupName
                    Description = aGroupDescription
                    Members = []
                }

                return group
            }

            match group with 
            | Ok g -> Ok g
            | Error error -> Error error


        else
            let msg = "Cannot provision"
            Error msg
  
        
             
       



    let provisionRole (aTenant : Tenant)(aRoleName :RoleName) (aRoleDescription: RoleDescription) : Result<Role, string> =

            let id = generateNoEscapeId();

            if aTenant.ActivationStatus = ActivationStatus.Activated then


                let role = result {

                    let! groupId = GroupId.create "group id" id
                    let! groupName = GroupName.create "group name" "INTERNAL_GROUP"
                    let! groupDescription = GroupDescription.create "group name" "INTERNAL_GROUP_DESCRIPTION"


                    let group = {
                        GroupId = groupId 
                        TenantId = aTenant.TenantId
                        Name = groupName
                        Description = groupDescription
                        Members = []
                    }


                    let role = result {

                        let id = generateNoEscapeId()
                        let! roleId = RoleId.create "role id" id


                        let role : Role = {
                            RoleId = roleId 
                            TenantId = aTenant.TenantId
                            Name = aRoleName
                            Description = aRoleDescription
                            SupportNesting = SupportNestingStatus.Support
                            InternalGroup = group
                    }

                    return role
                }


                    return! role
                }

                
                match role with 
                | Ok r -> Ok r
                | Error error -> Error error


            else
                let msg = "Cannot provision role"
                Error msg 







    let getAllAvailableRegistrationInvitation(aTenant:Tenant) : RegistrationInvitation list =

        match aTenant.ActivationStatus with 
        | ActivationStatus.Activated 
            -> aTenant.RegistrationInvitations
               |> List.filter RegistrationInvitations.isAvailableWithBakedDateTimeParam
        | ActivationStatus.Disactivated 
            -> []






    let getAllUnAvailableRegistrationInvitation(aTenant:Tenant) : RegistrationInvitation list =

        match aTenant.ActivationStatus with 
        | ActivationStatus.Activated 
            -> aTenant.RegistrationInvitations
               |> List.filter RegistrationInvitations.isNotAvailableWithBakedDateTimeParam
        | ActivationStatus.Disactivated 
            -> []



            


    let redfineRegistrationInvintationTimeSpan 
        (aTenant:Tenant)
        (aRegistrationInvitationId:RegistrationInvitationId)
        (aStartDate:DateTime)
        (anEndDate:DateTime)
        :Result<Tenant, string> =
        
         match aTenant.ActivationStatus with 

          | ActivationStatus.Activated ->

                let concernedRegistrationInvitation = 
                        aTenant.RegistrationInvitations
                        |> List.filter (fun nextRegistrationInvitation -> nextRegistrationInvitation.RegistrationInvitationId = aRegistrationInvitationId)
                        |> List.first

                match concernedRegistrationInvitation with 
                | Some invitation -> 

                    let remainingRegistrationInvitations = aTenant.RegistrationInvitations 
                                                           |> List.filter (fun nextRegistrationInvitation -> nextRegistrationInvitation.RegistrationInvitationId = aRegistrationInvitationId) 
                    let newInvitation = {invitation with StartingOn = aStartDate; Until = anEndDate}
                        
                    Ok {aTenant with RegistrationInvitations = remainingRegistrationInvitations@[newInvitation]}
                | None   
                    -> let msg = sprintf "Registration invitation with id %A not found" aRegistrationInvitationId
                       Error msg


          | ActivationStatus.Disactivated -> 
               let error = sprintf "Tenant is deactivated"
               Error error






    let registerUserForTenant
        (anInvitationId:RegistrationInvitationId)
        (aUSerName:Username)
        (aPassword:Password)
        (anEnablement:Enablement)
        (aPerson:Person)
        (aTenant:Tenant)
        : Result<User,string> =

        match aTenant.ActivationStatus with 
        | Activated ->
            
            match (isRegistrationInvitationAvailableThrough aTenant anInvitationId) with 
            | true ->

                    let rsCreateUser = result{

                        let strId = generateNoEscapeId()
                        let! userId = UserId.create "user id" strId 

                        let user = {UserId = userId; TenantId = aTenant.TenantId; Username = aUSerName; Password = aPassword; Enablement = anEnablement; Person = aPerson}

                        return user
                    }

                    match rsCreateUser with 
                    | Ok user -> 
                        Ok user
                    | Error error -> 
                        let msg = sprintf "Error occurred"
                        Error error
            | false ->
                let msg = sprintf "Registration expired/not available "
                Error msg
            
        | Disactivated ->
            let msg = sprintf "Tenant deactivated"
            Error msg





    let withdrawInvitation 
        (aTenant:Tenant)
        (aRegistrationInvitationId:RegistrationInvitationId)
        :Result<Tenant, string> =

        let optionalInvitationToWithdraw = 
                                    aTenant.RegistrationInvitations 
                                    |> List.filter (fun nextInvitation -> nextInvitation.RegistrationInvitationId = aRegistrationInvitationId )
                                    |> List.first

        match optionalInvitationToWithdraw with 
         | Some _ -> 

            let otherInvitations = aTenant.RegistrationInvitations 
                                    |> List.filter (fun nextInvitation -> not (nextInvitation.RegistrationInvitationId = aRegistrationInvitationId) )
            
            Ok {aTenant with RegistrationInvitations = otherInvitations }

         | None ->
            let msg = sprintf "No registration invitation found for identifier: %A" aRegistrationInvitationId
            Error msg







    let activateTenant (aTenant : Tenant) : Result<Tenant, string> =
            
            match aTenant.ActivationStatus with  
            | Disactivated ->
                Ok { aTenant with ActivationStatus = ActivationStatus.Activated }  
            | Activated -> 
                Error "Tenant already has its activation status set to Activated" 






    let deactivateTenant (aTenant : Tenant) : Result<Tenant, string> =
        
        match aTenant.ActivationStatus with  
        | Disactivated ->
            Error "Tenant already has its activation status set to Deactivated"
        | Activated -> 
            Ok { aTenant with ActivationStatus = ActivationStatus.Disactivated } 















module Role = 



    let create (roleId:string) (tenantId:string) (name:string) (description:string) : Result<Role,string> =
        
        let roleConstruct = result {

            let internalGroupId = generateNoEscapeId ()

            let! ids = roleId |> RoleId.create' 
            let! tenantIds = tenantId |> TenantId.create' 
            let! names = name |> RoleName.create'
            let! descriptions = description |> RoleDescription.create' 
            let! groupId = internalGroupId |> GroupId.create' 
            let! groupName = "INTERNAL_GROUP" |> GroupName.create'
            let! groupDescription = "INTERNAL_GROUP_DESCRIPTION" |> GroupDescription.create'


            let internalGroup = {
                GroupId = groupId 
                TenantId = tenantIds
                Name = groupName
                Description = groupDescription
                Members = []
                }
            
            let role:Role = {
                RoleId = ids
                TenantId = tenantIds
                Name = names
                Description = descriptions
                SupportNesting = SupportNestingStatus.Support
                InternalGroup = internalGroup
                }
            return role
        }

        roleConstruct















module User = 

    let create id tenantId first middle last email address primaryPhone secondaryPhone username password = 
        
        let tenantConstruct = result {

            let! ids = UserId.create "userId: " id
            let! tenantIds = TenantId.create "tenantId: " tenantId
            let! middleNames = MiddleName.create "middleName" middle
            let! firstNames = FirstName.create "FirstName" first
            let! lastNames = LastName.create "lastName" last
            let! emails = EmailAddress.create "email" email
            let! address = PostalAddress.create "address" address
            let! primaryPhone  = Telephone.create "primaryPhone" primaryPhone
            let! secondaryPhone  = Telephone.create "secondaryPhone" secondaryPhone
            let! username  = Username.create "username" username
            let! password  = Password.create "password" password

            let name = {First = firstNames; Middle = middleNames; Last = lastNames}
            let contactInfo = {Email = emails; Address = address; PrimaryTel = primaryPhone; SecondaryTel = secondaryPhone}
            let person = {Contact = contactInfo; Name = name; Tenant = tenantIds; User = ids}
            let enablement = {EnablementStatus = EnablementStatus.Enabled; StartDate = DateTime.Now; EndDate = DateTime.Now}
               
 
            return {
               UserId = ids
               TenantId = tenantIds
               Username = username
               Password = password
               Enablement = enablement
               Person = person
               }
        }

        tenantConstruct






    let changePassWord aUser aCurrentGivenPassword aNewGivenPassword = 
        if aUser.Password <> aCurrentGivenPassword then
            Error "Current Password not confirmed"
        else
            Ok {aUser with Password = aNewGivenPassword} 







    let changePersonalContactInformation aUser aContactInformation = 
        let personWithNewContact = {aUser.Person with Contact = aContactInformation}
        {aUser with Person = personWithNewContact}






    let changePersonalName aUser aFullName = 
        let personWithNewName = {aUser.Person with Name = aFullName}
        {aUser with Person = personWithNewName}






    let defineEnablement aUser anEnablement = 
        let userWithNewEnablement = {aUser with Enablement = anEnablement}
        userWithNewEnablement






    let isEnabled aUser  = 
        match aUser.Enablement.EnablementStatus with  
        | Enabled -> true
        | Disabled -> false






    let isDisabled aUser  = 
        match aUser.Enablement.EnablementStatus with  
        | Enabled -> false
        | Disabled -> true





    let toGroupMember (memberType:GroupMemberType) (aUser:User) : Result<GroupMember,string> =

        let rs = result {

                let enablement = aUser.Enablement 
                let! memberId = GroupMemberId.create "memberId" (UserId.value aUser.UserId)
                let! tenantId = TenantId.create "tenantId" (TenantId.value aUser.TenantId)
                let! name = GroupMemberName.create "groupName" (Username.value aUser.Username)
        
                let groupMember:GroupMember = {
                        MemberId = memberId
                        TenantId = tenantId
                        Name = name
                        Type = memberType
                    }

                return groupMember
        }
        match rs with
        | Ok rs -> Ok rs
        | Error error -> Error error 






    let toUserDesriptor (aUser:User): Result<UserDescriptor, string> =

        let rsUserDercriptor = result {

            let! userDescriptorId = UserDescriptorId.create "user descriptor id : " (UserId.value aUser.UserId)

            return {
                UserDescriptorId = userDescriptorId
                TenantId = aUser.TenantId
                Username = aUser.Username
                Email = aUser.Person.Contact.Email
                }
        }

        rsUserDercriptor

            
        
    
module GroupMembers = 


///Some global transformer functions
/// 
/// 
/// 
/// 
/// 
/// 
/// 

    let preppend  firstR restR = 
        match firstR, restR with
        | Ok first, Ok rest -> Ok (first::rest)  
        | Error error1, Ok _ -> Error error1
        | Ok _, Error error2 -> Error error2  
        | Error error1, Error _ -> Error error1





    let ResultOfSequenceTemp aListOfResults =
        let initialValue = Ok List.empty
        List.foldBack preppend aListOfResults initialValue







    ///Dependency function for the create one                    
    let transformGroupMemberDtoTo (nextGroupMemberDtoTemp:GroupMemberDtoTemp):Result<GroupMember,string> =
        
       let rsTransfromation = result {

          let! tenantId = TenantId.create' nextGroupMemberDtoTemp.TenantId
          let! name = GroupMemberName.create' nextGroupMemberDtoTemp.Name
          let! memberId = GroupMemberId.create' nextGroupMemberDtoTemp.MemberId

          let rsGroupMember:GroupMember =  {
              MemberId = memberId
              TenantId = tenantId
              Name = name
              Type = GroupGroupMember
              }  
        
        return rsGroupMember

       }
       rsTransfromation



    let create fieldName (groupMemberDtoTempList:GroupMemberDtoTemp list) =  
        groupMemberDtoTempList
        |> List.map transformGroupMemberDtoTo
        |> ResultOfSequenceTemp


    let create' = create "GroupMembers : "



module Group = 





    let toMemberOfTypeGroup memberType aGroup =

        let resultOfGroupMember = result {
                
            let! memberId = aGroup.GroupId 
                            |> GroupId.value  
                            |> GroupMemberId.create' 

            let! tenantId = aGroup.TenantId 
                            |> TenantId.value 
                            |> TenantId.create'

            let! name = aGroup.Name 
                        |> GroupName.value 
                        |> GroupMemberName.create'
    
            let rsOfGroupGroupMember:GroupMember = {
                    MemberId = memberId
                    TenantId = tenantId
                    Name = name
                    Type = memberType 
                    }

            return rsOfGroupGroupMember
        }

        resultOfGroupMember











    let toMemberOfTypeUser memberType aUser =

        let resultOfGroupMember = result {
                
            let! memberId = aUser.UserId 
                            |> UserId.value  
                            |> GroupMemberId.create' 

            let! tenantId = aUser.TenantId 
                            |> TenantId.value 
                            |> TenantId.create'

            let! name = aUser.Username 
                        |> Username.value 
                        |> GroupMemberName.create'
    
            let rsOfUserGroupMember:GroupMember = {
                    MemberId = memberId
                    TenantId = tenantId
                    Name = name
                    Type = memberType 
                    }

            return rsOfUserGroupMember
        }

        resultOfGroupMember




    let toMemberOfTypeGroup' =  toMemberOfTypeGroup  GroupMemberType.GroupGroupMember
    let toMemberOfTypeUser' =  toMemberOfTypeUser  GroupMemberType.UserGroupMember















    let create id tenantId name description members = 
        
        let rsCreateGroup = result {

            let! groupId = id |> GroupId.create'  
            let! tenantId' = tenantId |> TenantId.create'
            let! name' = name |> GroupName.create'
            let! description' = description |> GroupDescription.create' 
            let! members = members |> GroupMembers.create'
          

            return {
               GroupId = groupId
               TenantId = tenantId'
               Name = name'
               Description = description'
               Members = members
            }
        }

        rsCreateGroup







    let changePassWord aUser aCurrentGivenPassword aNewGivenPassword = 
        if aUser.Password <> aCurrentGivenPassword then
            Error "Current Password not confirmed"
        else
            Ok {aUser with Password = aNewGivenPassword} 







    let addGroupToGroup (aGroupToAddTo:Group)(aGroupToAdd:Group)(isGroupMemberService: IsGroupMember') : Result<Group, string> =

        let doBothGroupsHaveSameTenant = (aGroupToAddTo.TenantId = aGroupToAdd.TenantId)
        let isNotTheSameGroup  = not (aGroupToAddTo.GroupId = aGroupToAdd.GroupId)
        let toGroupMember' = toMemberOfTypeGroup GroupGroupMember
        let isGroupMemberService' = isGroupMemberService aGroupToAddTo 

        match doBothGroupsHaveSameTenant && isNotTheSameGroup with 
        | true -> 

            let rsGrouMember = result {

                let! aMemberToAdd = aGroupToAdd 
                                    |> toGroupMember'
                                    
                let isTheGroupMemberToAddAlreadyAMember = aMemberToAdd 
                                                          |> isGroupMemberService' 

                return   isTheGroupMemberToAddAlreadyAMember
            }

            match rsGrouMember with 
            | Ok isAlreadyGroupMember -> 

                if not isAlreadyGroupMember then
                    
                    let newMembers =  aGroupToAddTo.Members
                    let rsIsGrouMemberToAdd = result {
                            let! aMemberToAdd = aGroupToAdd |> toGroupMember'
                            return aMemberToAdd
                        }

                    match rsIsGrouMemberToAdd with 
                    | Ok groupMember -> 
                        let group = {aGroupToAddTo with Members = [groupMember]@newMembers}
                        Ok group
                    | Error error ->
                        Error error
                else
                     Error "Group already a member"

            | Error isNotGoupMemberYet  -> 
                 Error isNotGoupMemberYet
           

        | false -> 
            let msg = sprintf "Wrong tenant consistency"
            Error msg

        




    
    let addUserToGroup (aGroupToAddTo:Group) (aUserToAdd:User)  =



        let AreFromSameTenant = (aGroupToAddTo.TenantId = aUserToAdd.TenantId)
        let UserIsEnabled = aUserToAdd |> User.isEnabled
   
        match AreFromSameTenant && UserIsEnabled with  
        | true -> 

            let isUseGroupMember = result {

                let! groupMemberToAdd = aUserToAdd |> toMemberOfTypeUser'

                let searchGivenMemberInGroupMembers = aGroupToAddTo.Members
                                                      |> List.filter (
                                                          fun next -> 
                                                            groupMemberToAdd.MemberId = next.MemberId
                                                      )

                let isUseGroupMember = match searchGivenMemberInGroupMembers with 
                                       | [] -> false
                                       | [_] -> true
                                       | head::tail -> false 

                return   isUseGroupMember
            }

            
            
           
            match isUseGroupMember with  
            | Ok rs -> 

                match  rs with 
                | false ->
                    
                    let groupMemberToAdd = result {

                        let! groupMemberToAdd = aUserToAdd |> toMemberOfTypeUser'

                        return groupMemberToAdd
                    }

                    match groupMemberToAdd with  
                    | Ok grouMember -> 

                        Ok {aGroupToAddTo with Members = aGroupToAddTo.Members@[grouMember]}

                    | Error error -> 
                    
                        Error error
                | true ->

                    let msg = sprintf "Already group member" 
                    Error msg

            | Error _ -> 
                let msg = sprintf "Wrong tenant consistency 2" 
                Error msg

        | false ->
            let msg = sprintf "Wrong tenant consistency or disabled user account" 
            Error msg
  

            

        
        
            
               


 


  
    
 


