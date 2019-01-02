module IdentityAndAcccess.DomainTypes.Functions

open System
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open FSharp.Data.Sql
open IdentityAndAcccess.DomainTypes




let generateNoEscapeId () = Guid.NewGuid().ToString().Replace("-", "")






module RegistrationInvitations =

    let fromRegistrationInvitationDtoTempToDomain = 
        fun aInvitationDto -> result{
                                                  let! invitationId = RegistrationInvitationId.create "invitation id: " aInvitationDto.RegistrationInvitationId
                                                  let! tenantId = TenantId.create "tenantId" aInvitationDto.TenantId
                                                  let! desciption = RegistrationInvitationDescription.create "tenant description" aInvitationDto.Description
                                                  let startingOn =  aInvitationDto.StartingOn
                                                  let until = aInvitationDto.Until


                                                  let p:RegistrationInvitation = {
                                                      RegistrationInvitationId = invitationId
                                                      TenantId = tenantId
                                                      Description = desciption
                                                      StartingOn = startingOn
                                                      Until = until
                                                  }  

                                                  return p
                                       }
                                      
                    
    let create fieldName (invitationsDto:RegistrationInvitationDtoTemp list) =  
        invitationsDto
        |> List.map fromRegistrationInvitationDtoTempToDomain

    (*let fromRegInvToRegInvTemp fieldName (invitations:RegistrationInvitation list) =  
        invitations
        |> List.map (fun anInvitation -> 

                                       result{
                                                  let invitationId = RegistrationInvitationId.value anInvitation.RegistrationInvitationId
                                                  let tenantId = TenantId.value anInvitation.TenantId
                                                  let desciption = RegistrationInvitationDescription.value anInvitation.Description
                                                  let startingOn =  anInvitation.StartingOn
                                                  let until = anInvitation.Until


                                                  let p:RegistrationInvitationDto = {
                                                      RegistrationInvitationId = invitationId
                                                      TenantId = tenantId
                                                      Description = desciption
                                                      StartingOn = startingOn
                                                      Until = until
                                                  }  

                                                  return p
                                       }
                                      
                    )

    *)
    let isIdentifiedBy (aRegistrationInvitationId : RegistrationInvitationId)(aRegistrationInvitation : RegistrationInvitation) : Boolean = 
        let result = (aRegistrationInvitation.RegistrationInvitationId = aRegistrationInvitationId)

        if result then
            result
        else
            (RegistrationInvitationId.value aRegistrationInvitationId  = RegistrationInvitationDescription.value aRegistrationInvitation.Description)

    let invitation  (aRegistrationInvitationId : RegistrationInvitationId) (aRegistrationInvitationList : RegistrationInvitation list) =
        aRegistrationInvitationList 
        |> List.filter (fun nextRegistrationInvitation -> isIdentifiedBy aRegistrationInvitationId nextRegistrationInvitation)
        |> List.tryHead

    let isAvailable (aRegistrationInvitation : RegistrationInvitation) : Boolean =

        let now = DateTime.Now
        let resultCompareNowAndSartDate = DateTime.Compare (now, aRegistrationInvitation.StartingOn)
        let resultCompareUntilAndEnd = DateTime.Compare (now, aRegistrationInvitation.Until)
        
        if (resultCompareNowAndSartDate < 0 || resultCompareUntilAndEnd < 0)  then 
            false
        else 
            true
          








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
            | Some r -> true
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

                    let newTenant = { aTenant with RegistrationInvitations = [registrationInvitation]@ aTenant.RegistrationInvitations}

                    return newTenant 
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
            let tenantId = (TenantId.value aTenant.TenantId)

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
                            Group = group
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

module Role = 



    let create (id:string) (tenantId:string) (name:string) (description:string) : Result<Role,string> =
        
        let roleConstruct = result {


                    let internalGroupId = generateNoEscapeId ()

                    let! ids = RoleId.create "role id: " id
                    let! tenantIds = TenantId.create "tenant id: " tenantId
                    let! names = RoleName.create "role name: " name
                    let! descriptions = RoleDescription.create "role description: " description



                    let! groupId = GroupId.create "group id" internalGroupId
                    let! groupName = GroupName.create "group name" "INTERNAL_GROUP"
                    let! groupDescription = GroupDescription.create "group name" "INTERNAL_GROUP_DESCRIPTION"


                    let group = {
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
                            Group = group
                    }

                    return role
                }
        roleConstruct















module User = 

    let create id tenantId first middle last email address primaryPhone secondaryPhone username password = 
        
        result {

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
        aUser.Enablement.EnablementStatus = Enabled

    let isDisabled aUser  = 
        aUser.Enablement.EnablementStatus = Disabled

    let toGroupMember memberType aUser =

        result {
                let enablement = aUser.Enablement 
                
                let! memberId = GroupMemberId.create "memberId" (UserId.value aUser.UserId)
                let! tenantId = TenantId.create "tenantId" (TenantId.value aUser.TenantId)
                let! name = GroupMemberName.create "groupName" (Username.value aUser.Username)
        
                let p:GroupMember = {MemberId = memberId; TenantId = tenantId; Name = name; Type = memberType }

                return p
        }
    
module GroupMembers = 

    let create fieldName (membersDto:GroupMemberDtoTemp list) =  
        membersDto
        |> List.map (fun aMemberDto -> 

                                       result{
                                                  let! memberId = GroupMemberId.create "meberId" aMemberDto.MemberId
                                                  let! tenantId = TenantId.create "tenantId" aMemberDto.TenantId
                                                  let! name = GroupMemberName.create "groupMemberName" aMemberDto.Name
                                                  let! memberId = GroupMemberId.create "meberId" aMemberDto.MemberId


                                                  let p:GroupMember = {
                                                      MemberId = memberId
                                                      TenantId = tenantId
                                                      Name = name
                                                      Type = GroupMemberType.Group
                                                  }  

                                                  return p
                                       }
                                      
                    )

[<RequireQualifiedAccess>]
module Group = 


 
    let toGroupMember memberType aGroup =

        result {
                
                let! memberId = GroupMemberId.create "memberId" (GroupId.value aGroup.GroupId)
                let! tenantId = TenantId.create "tenantId" (TenantId.value aGroup.TenantId)
                let! name = GroupMemberName.create "groupName" (GroupName.value aGroup.Name)
        
                let p:GroupMember = {MemberId = memberId; TenantId = tenantId; Name = name; Type = memberType }

                return p
        }


    let create id tenantId name description (members: GroupMemberDtoTemp list) = 
        
        result {

            let! ids = GroupId.create "groupId: " id
            let! tenantIds = TenantId.create "tenantId: " tenantId
            let! names = GroupName.create "groupName" name
            let! descriptions = GroupDescription.create "groupDescription" description
            let members = GroupMembers.create "groupMembers" members
          

            return {
                       GroupId = ids
                       TenantId = tenantIds
                       Name = names
                       Description = descriptions
                       Members = []
            }
        }

    let changePassWord aUser aCurrentGivenPassword aNewGivenPassword = 
        if aUser.Password <> aCurrentGivenPassword then
            Error "Current Password not confirmed"
        else
            Ok {aUser with Password = aNewGivenPassword} 


    let addGroupToGroup aGroupToAddTo aGroupToAdd groupMemberService =
        ()
    
    let addUserToGroup (aGroupToAddTo:Group) (aUserToAdd:User) (aGroupMember:GroupMember) =
       
                if not (aGroupToAddTo.TenantId = aUserToAdd.TenantId) then 
                    Error "Tenant mismatch"
                elif (User.isDisabled aUserToAdd) then  
                    Error "User is disabled"
                else 
                    let group = {aGroupToAddTo with Members =  aGroupMember :: aGroupToAddTo.Members}
                    Ok group
               
module Services =


    type GetUserById = UserId -> Result<User, string>
    type GetGroupById = GroupId -> Result<Group, string>
    type GetGroupMemberById = GroupMemberId -> Result<Group, string>
    type IsGroupMember = Group -> GroupMember -> GetGroupMemberById -> Boolean
    type IsUserInNestedGroup = Group -> User -> GetGroupMemberById -> Boolean


     //Tenant type related services 

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
             

    //User type related services 
    let confirmUser aGroup aUser (getUserById:GetUserById)  =      

            let user = getUserById aUser.UserId

            match user with 
            | Ok user -> User.isEnabled user
            | Error error -> false

 


  
    //Group type related services
    let isGroupMember :IsGroupMember = 
        fun aGroup aMember getGroupMemberById ->
        
            let rec recIGroupMember  
                (aMember : GroupMember) 
                (getGroupMemberById : GetGroupMemberById)
                (aGroupMemberList : GroupMember list) =
                match aGroupMemberList with
                | [] -> 
                    false
                | head::tail ->
                    if (head.Type = GroupMemberType.Group) && (head = aMember) then
                        true
                    else 
                    ///IO operation here. looking for a group member by its group member identifier 
                        let additionalGroupToSearch = getGroupMemberById head.MemberId
                    ///IO operation here.
                        match additionalGroupToSearch with
                        | Ok anAdditionalGroupToSearch ->

                            let newMembersToAppend =  anAdditionalGroupToSearch.Members
                            let allMembers = tail@newMembersToAppend
                            let result = recIGroupMember aMember getGroupMemberById allMembers
                            result

                        | Error _ -> false

            recIGroupMember  aMember  getGroupMemberById   aGroup.Members    
                         

 


