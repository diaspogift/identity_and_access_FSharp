module IdentityAndAcccess.DomainTypes.Functions

open System
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Role
open FSharp.Data.Sql
open IdentityAndAcccess.DomainTypes
open MongoDB.Bson






let unwrapToStandardGroup aGroupToUnwrapp = 
        match aGroupToUnwrapp with 
            | Standard aStandardGroup -> aStandardGroup
            | Internal anInternalGroup -> anInternalGroup



let generateNoEscapeId () =   
    let objectId = ObjectId.GenerateNewId()
    objectId.ToString()



type IsGroupMemberService =  Group -> GroupMember  -> Boolean


///Helper functionsssss
/// 
/// 
/// 
/// 
/// 
/// 
module DomainHelpers =
    let unwrapToStandardGroup aGroupToAddToUnwrapp = 
        match aGroupToAddToUnwrapp with 
            | Standard aStandardGroup -> aStandardGroup
            | Internal anInternalGroup -> anInternalGroup


///Services that act on domain types
/// 
/// 
/// 
/// 
/// 
/// 
/// 

module ServiceInterfaces =




     ///This module design the interface for the communication between the domain and the infrastructural one
     /// 
     /// 
     /// 
     /// 
    


    //Others

    type CallerCredential = CallerCredential of string






















    ///Password type related services
    type PasswordEncryptionService = 
            StrongPassword -> Result<EncrytedPassword, string>

    
    type PasswordsMatcherService = 
            Password -> EncrytedPassword -> Boolean

    type GeneratePasswordService = 
            Password -> Result<StrongPassword, string>



    




    //Database dependencies interfaces to be used by the domain services
    type LoadGroupById = 
            GroupId -> Result<Group, string>


    type LoadGroupMemberById = 
            GroupMemberId -> Result<Group, string>


    type LoadUserByUserIdPasswordAndTenantId = 
            UserId -> EncrytedPassword -> TenantId -> Result<User, string>


    type LoadTenantById = 
            TenantId -> Result<Tenant, string>

    type UpdateOneTenant = 
            Tenant -> Result<unit, string>

    type LoadUserByUserIdAndTenantId = 
            UserId -> TenantId -> Result<User,string>

    type SaveOneTenant =          
            Tenant -> Result<unit, string>

    type SaveOneUser =          
            User -> Result<unit, string>

    type SaveOneRole =          
            Role -> Result<unit, string>



    //Domaim services dependencies interfaces for domain business logic to be used by the
    //group member services
    
    type IsGroupMemberService = 
            Group 
                -> GroupMember  
                -> Boolean




    type IsGroupMemberServiceWithBakedGetGroupMemberByIdDependency = 
            Group 
                -> GroupMember   
                -> Boolean




    type IsUserInNestedGroupService = 
            LoadGroupMemberById 
                -> Group 
                -> User 
                -> Boolean




    type IsUserInNestedGroupService' = 
             Group 
                -> User 
                -> Boolean



    type ConfirmUserServive = 
            LoadUserByUserIdAndTenantId 
                -> Group 
                -> User
                -> Boolean


    
    type ConfirmUserServive' = 
             Group 
                -> User 
                -> Boolean  


             

    type StrongPasswordGeneratorService = 
            Password -> Result<StrongPassword, string>




    type ProvisionTenantService = 
               StrongPasswordGeneratorService
                -> PasswordEncryptionService
                -> TenantName 
                -> TenantDescription 
                -> FirstName 
                -> MiddleName 
                -> LastName 
                -> EmailAddress 
                -> PostalAddress 
                -> Telephone 
                -> Telephone
                ->  Result<Provision,string>






    type ProvisionTenantService' = 
            TenantName 
                -> TenantDescription 
                -> FullName 
                -> EmailAddress 
                -> PostalAddress 
                -> Telephone 
                -> Telephone 
                -> Result<Tenant,string>







    type GroupMemberServices = {

        TimeServiceWasCalled: DateTime
        CallerCredentials: CallerCredential

        isGroupMember: IsGroupMemberService
        isUserInNestedGroup : IsUserInNestedGroupService
  

    }


    ///User type related services
    

    type IsUserInRoleService =  
        IsUserInNestedGroupService 
            -> ConfirmUserServive
            -> Role
            -> User
            -> Boolean
            

    type AuthenticationService = 
            PasswordsMatcherService
                -> IsUserInRoleService
                -> User
                -> Tenant 
                -> Password 
                -> Result<UserDescriptor,string>






    

    type IsUserInRoleService' =  
        IsUserInNestedGroupService' 
            -> ConfirmUserServive'
            -> Role
            -> User
            -> Boolean


    type Subject = Subject of string
    type Content = Content of string
    type EmailSentAknowledgment = 
        | Sent of DateTime
        | NotSent


    type EmailSenderService =
        EmailAddress 
            -> EmailAddress
            -> Subject
            -> Content
            -> EmailSentAknowledgment





  



    

    
    
             

 









///Function that act on Domain types


module RegistrationInvitations =

    let fromRegistrationInvitationDtoTempToDomain = 
        fun (aInvitationDto:RegistrationInvitationDtoTemp) ->                                     
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







module ContactInformation =



    let fullCreate anEmailAddress aPostalAddress aPrimaryTel aSecondaryTel =
        {Email = anEmailAddress; Address = aPostalAddress; PrimaryTel = aPrimaryTel; SecondaryTel = aSecondaryTel}

    let changeEmailAddress (aContactInformation:ContactInformation) anEmailAddress =
        {aContactInformation with Email = anEmailAddress}
  
    let changePostalAddress (aContactInformation:ContactInformation) aPostalAddress =
        {aContactInformation with Address = aPostalAddress}

    let changePrimaryTelephone (aContactInformation:ContactInformation) aPrimaryTel =
        {aContactInformation with PrimaryTel = aPrimaryTel}

    let changeSecondaryTelephone (aContactInformation:ContactInformation) aSecondaryTel =
        {aContactInformation with SecondaryTel = aSecondaryTel}    

    
 









module Tenant = 







    ///Helper functions
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


    open ContactInformation






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






    let fullCreate id name description activationStatus (regInvDtoList:RegistrationInvitationDtoTemp array) = 


        let fromRegIncDtoTempToRegInv (aRegIncDtoTemp : RegistrationInvitationDtoTemp) = 

            let rsRegistrationInvitation = result {

                let! invId = aRegIncDtoTemp.RegistrationInvitationId |> RegistrationInvitationId.create'
                let! desc = aRegIncDtoTemp.Description |> RegistrationInvitationDescription.create'
                let! tenantId = aRegIncDtoTemp.TenantId |> TenantId.create'
                let startingOn = aRegIncDtoTemp.StartingOn 
                let until = aRegIncDtoTemp.Until

                let invitation : RegistrationInvitation = {

                    RegistrationInvitationId = invId
                    Description = desc
                    TenantId = tenantId
                    StartingOn = startingOn
                    Until = until
                }

                return invitation
            }

            rsRegistrationInvitation

            

            



        
        result {

            let! ids = TenantId.create "tenant id: " id
            let! names = TenantName.create "tenant name: " name
            let! descriptions = TenantDescription.create "tenant description: " description

            
            let! registrationInvitations = 
                regInvDtoList
                |> Array.map fromRegIncDtoTempToRegInv
                |> Array.toList
                |> ResultOfSequenceTemp

            
            return {
                    TenantId = ids
                    Name = names
                    Description = descriptions
                    RegistrationInvitations = registrationInvitations
                    ActivationStatus = activationStatus
            }
        }




    let createFullActivatedTenant (name:TenantName) (description:TenantDescription) : Result<Tenant,string> = 
        
        result {

            let strTenantId = generateNoEscapeId()

            let! tenantId = TenantId.create' strTenantId
            
            return {
                    TenantId = tenantId
                    Name = name
                    Description = description
                    RegistrationInvitations = []
                    ActivationStatus = ActivationStatus.Activated
            }
        }





    let isRegistrationInvitationAvailableThrough (aTenant : Tenant) (aRegistrationInvitationId : RegistrationInvitationId) : Boolean =
        
        match aTenant.ActivationStatus with 
        | Activated -> 
            let result = RegistrationInvitations.invitation aRegistrationInvitationId aTenant.RegistrationInvitations
            match result with  
            | Some _ -> true
            | None -> false
        | Deactivated 
            -> false
        




    let offerRegistrationInvitation (aTenant : Tenant) (aDescription : RegistrationInvitationDescription) : Result<Tenant*RegistrationInvitation, string> =
        
          
        if aTenant.ActivationStatus = ActivationStatus.Activated then 
            
            result {

                    let id = generateNoEscapeId()

                    let! registrationInvitationId = 
                        id 
                        |> RegistrationInvitationId.create' 

                    let registrationInvitation : RegistrationInvitation = {
                            Description = aDescription
                            RegistrationInvitationId = registrationInvitationId
                            StartingOn = DateTime.Now
                            TenantId = aTenant.TenantId
                            Until = DateTime.Now
                            }

                    let tenantWithNewRegistrationInvitation = { aTenant with RegistrationInvitations = [registrationInvitation]@aTenant.RegistrationInvitations}

                    return (tenantWithNewRegistrationInvitation, registrationInvitation)
                }
        else 
            let msg =  "Tenant activation status is deactivated" 
            Error  msg





    let provisionGroup (aTenant : Tenant)(aGroupName :GroupName) (aGroupDescription: GroupDescription) : Result<Group, string> =

        let id = generateNoEscapeId();

        if aTenant.ActivationStatus = ActivationStatus.Activated then

            let group = result {

                let! groupId = GroupId.create' id

                let group = Standard {
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


                    let group = Internal {
                        GroupId = groupId 
                        TenantId = aTenant.TenantId
                        Name = groupName
                        Description = groupDescription
                        Members = []
                    }


                    let role = result {

                        let id = generateNoEscapeId()
                        let! roleId = RoleId.create "role id" id

                        ///TODO MAKE THIS CONSTRUT PRIVATE
                        let role : Role = {
                            RoleId = roleId 
                            TenantId = aTenant.TenantId
                            Name = aRoleName
                            Description = aRoleDescription
                            SupportNesting = SupportNestingStatus.Support
                            InternalGroup =  group
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
        | Deactivated 
            -> []






    let getAllUnAvailableRegistrationInvitation(aTenant:Tenant) : RegistrationInvitation list =

        match aTenant.ActivationStatus with 
        | ActivationStatus.Activated 
            -> aTenant.RegistrationInvitations
               |> List.filter RegistrationInvitations.isNotAvailableWithBakedDateTimeParam
        | Deactivated 
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


          | Deactivated -> 
               let error = sprintf "Tenant is deactivated"
               Error error






    let registerUserForTenant
        (aTenant:Tenant)
        (anInvitationId:RegistrationInvitationId)
        (aUSerName:Username)
        (aPassword:Password)
        (anEnablement:Enablement)

        (anEmailAddress:EmailAddress)
        (aPostalAddress:PostalAddress)
        (aPrimaryTel:Telephone)
        (aSecondaryTel:Telephone)

        (aFirstName:FirstName)
        (aMiddleNAme:MiddleName)
        (aLastName:LastName)


        : Result<User,string> =



               

        match aTenant.ActivationStatus with 
        | Activated ->
            
            match (isRegistrationInvitationAvailableThrough aTenant anInvitationId) with 
            | true ->

                    let rsCreateUser = result{


                        let contactInfo = ContactInformation.fullCreate anEmailAddress aPostalAddress aPrimaryTel aSecondaryTel
                        let strUseId = generateNoEscapeId()
                        let! userId = UserId.create' strUseId
                        let fullName = {First = aFirstName; Middle =  aMiddleNAme; Last = aLastName}

                        let person = {Contact = contactInfo; Name = fullName; Tenant = aTenant.TenantId; User = userId}


                        ///TODO make sure I build a constructor in the way that they will always be consistency between the userid in user and person
                        let user = {UserId = userId; TenantId = aTenant.TenantId; Username = aUSerName; Password = aPassword; Enablement = anEnablement; Person = person}

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
            
        | Deactivated ->
            let msg = sprintf "Tenant deactivated"
            Error msg





    let withdrawRegistrationInvitation 
        (aTenant:Tenant)
        (aRegistrationInvitationId:RegistrationInvitationId)
        :Result<Tenant * RegistrationInvitation, string> =

        let optionalInvitationToWithdraw = 
                                    aTenant.RegistrationInvitations 
                                    |> List.filter (fun nextInvitation -> nextInvitation.RegistrationInvitationId = aRegistrationInvitationId )
                                    |> List.first

        match optionalInvitationToWithdraw with 
         | Some reg -> 

            let otherInvitations = aTenant.RegistrationInvitations 
                                    |> List.filter (fun nextInvitation -> not (nextInvitation.RegistrationInvitationId = aRegistrationInvitationId) )
            
            Ok ({aTenant with RegistrationInvitations = otherInvitations }, reg)

         | None ->
            let msg = sprintf "No registration invitation found for identifier: %A" aRegistrationInvitationId
            Error msg







    let activateTenant (aTenant : Tenant) : Result<Tenant, string> =


            printfn "Here is the passed tenant ========== %A" aTenant
     
            match aTenant.ActivationStatus with  
            | Deactivated ->
                Ok { aTenant with ActivationStatus = ActivationStatus.Activated }  
            | Activated -> 
                Error "Tenant already has its activation status set to Activated" 






    let deactivateTenant (aTenant : Tenant) : Result<Tenant, string> =
        
        match aTenant.ActivationStatus with  
        | Deactivated ->
            Error "Tenant already has its activation status set to Deactivated"
        | Activated -> 
            Ok { aTenant with ActivationStatus = Deactivated } 























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

            let name = {First = firstNames; Middle =  middleNames; Last = lastNames}
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



    let toUserGroupMember = toGroupMember GroupMemberType.UserGroupMember



    let toUserDesriptor (aUser:User): Result<UserDescriptor, string> =

        let rsUserDercriptor = result {

            let! userDescriptorId = UserDescriptorId.create "user descriptor id : " (UserId.value aUser.UserId)

            return {
                UserDescriptorId = userDescriptorId
                TenantId = aUser.TenantId
                Username = aUser.Username
                Email = aUser.Person.Contact.Email
                Roles = []
                }
        }

        rsUserDercriptor





























         













module DateTimeSpan =



    let create fieldName startDate endDate = 
        ConstrainedType.createDatetimeSpan fieldName startDate endDate

    let create' = create  "Datetime Span : "



    let startDate aDateTimeSpan = aDateTimeSpan.Start
    let endDate aDateTimeSpan = aDateTimeSpan.End








module Enablement = 

    let fullCreate (aStartDate:DateTime) (endDate:DateTime) (anEnablementStatus:EnablementStatus)  = 

        let rsEnablement = result {

            let! dateSpan = DateTimeSpan.create' aStartDate endDate

            let enablement = {
                EnablementStatus = anEnablementStatus
                StartDate = dateSpan.Start
                EndDate = dateSpan.End
            }

            return enablement
        }

        rsEnablement
        
        
        


















module GroupMember = 





    let isOfTypeUser (aGroupMember:GroupMember) : Boolean =
        match aGroupMember.Type with 
            | GroupMemberType.UserGroupMember -> true
            | GroupMemberType.GroupGroupMember -> false




    let isOfTypeGroup (aGroupMember:GroupMember) : Boolean =
        match aGroupMember.Type with 
            | GroupMemberType.GroupGroupMember -> true
            | GroupMemberType.UserGroupMember -> false
















        
    
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




 

    open ServiceInterfaces




    let areGroupsFromSameTenants (firstGroup:Group) (secondGroup:Group) = 
        let uF = unwrapToStandardGroup firstGroup
        let uS = unwrapToStandardGroup secondGroup

        uF.TenantId = uS.TenantId   

    
    let areGroupAndUserFromSameTenants (aGroup:Group) (aUser:User) = 
        let uG = unwrapToStandardGroup aGroup
        uG.TenantId = aUser.TenantId   




    let toMemberOfTypeGroup (memberType:GroupMemberType) (aGroup:Group) =

        let resultOfGroupMember = result {

            let aStandardGroup =  aGroup 
                                  |> DomainHelpers.unwrapToStandardGroup
                
            let! memberId = aStandardGroup.GroupId 
                            |> GroupId.value  
                            |> GroupMemberId.create' 

            let! tenantId = aStandardGroup.TenantId 
                            |> TenantId.value 
                            |> TenantId.create'

            let! name = aStandardGroup.Name 
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











    let toMemberOfTypeUser memberType (aUser:User) =

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
          

            return Standard {
               GroupId = groupId
               TenantId = tenantId'
               Name = name'
               Description = description'
               Members = members
            }
        }

        rsCreateGroup




    let createFull (members : GroupMember list) (groupId:GroupId) (tenantId:TenantId) (name:GroupName) (description:GroupDescription)  = 
        
        let rsCreateGroup = result {

            return Standard {
               GroupId = groupId
               TenantId = tenantId
               Name = name
               Description = description
               Members = members
            }
        }

        rsCreateGroup

        
   


    let changePassWord aUser aCurrentGivenPassword aNewGivenPassword = 
        if aUser.Password <> aCurrentGivenPassword then
            Error "Current Password not confirmed"
        else
            Ok {aUser with Password = aNewGivenPassword} 



    let isStandardGroup aGroup = match aGroup with 
                                 | Standard g -> true
                                 | Internal g -> false   



    let isInternalGroup aGroup = match aGroup with 
                                 | Standard g -> false
                                 | Internal g -> true   





    let addGroupToGroup (aGroupToAddTo:Group)(aGroupToAdd:Group)(isGroupMemberService: IsGroupMemberService) : Result<Group, string> =

        let unwrappedGroupToAddTo = match aGroupToAddTo with 
                                        | Standard aStandardGroup -> aStandardGroup
                                        | Internal anInternalGroup -> anInternalGroup
                                        

        let doBothGroupsHaveSameTenant = (unwrappedGroupToAddTo.TenantId = unwrappedGroupToAddTo.TenantId)
        let isNotTheSameGroup  = not (unwrappedGroupToAddTo.GroupId = unwrappedGroupToAddTo.GroupId)
        let toGroupGroupMember = toMemberOfTypeGroup GroupGroupMember
        let isGroupMemberService' = isGroupMemberService aGroupToAddTo 
        let isGroupToAddToRoleNotInterNalGroup =    aGroupToAddTo 
                                                    |> isInternalGroup 
                                                    |> not


        match doBothGroupsHaveSameTenant && isNotTheSameGroup && isGroupToAddToRoleNotInterNalGroup with 
        | true -> 

            let rsGrouMember = result {

                let! aMemberToAdd = aGroupToAdd 
                                    |> toGroupGroupMember
                                    
                let isTheGroupMemberToAddAlreadyAMember = aMemberToAdd 
                                                          |> isGroupMemberService' 

                return   isTheGroupMemberToAddAlreadyAMember
            }

            match rsGrouMember with 
            | Ok isAlreadyGroupMember -> 

                if not isAlreadyGroupMember then
                    
                    
                    let aStandardGroupToAdd =  aGroupToAddTo 
                                               |> DomainHelpers.unwrapToStandardGroup


                    let newMembers =  aStandardGroupToAdd.Members

                    let rsIsGrouMemberToAdd = result {
                        let! aMemberToAdd = aGroupToAdd
                                            |> toGroupGroupMember
                        return aMemberToAdd
                        }

                    match rsIsGrouMemberToAdd with 
                    | Ok groupMember -> 
                        let group = {aStandardGroupToAdd with Members = [groupMember]@newMembers}
                        Ok (Standard group)
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


        let aStandardGroupToAdd =  aGroupToAddTo 
                                               |> DomainHelpers.unwrapToStandardGroup

        let AreFromSameTenant = (aStandardGroupToAdd.TenantId = aUserToAdd.TenantId)
        let UserIsEnabled = aUserToAdd |> User.isEnabled
        let isGroupToAddToRoleNotInterNalGroup =    aGroupToAddTo 
                                                    |> isInternalGroup 
                                                    |> not        
   
        match AreFromSameTenant && UserIsEnabled && isGroupToAddToRoleNotInterNalGroup with  
        | true -> 

            let isUseGroupMember = result {

                let! groupMemberToAdd = aUserToAdd |> toMemberOfTypeUser'

                let searchGivenMemberInGroupMembers = aStandardGroupToAdd.Members
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

                        Ok {aStandardGroupToAdd with Members = aStandardGroupToAdd.Members@[grouMember]}

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
  

            

    let isMember (isUserInNestedGroupService:IsUserInNestedGroupService') //Service depndency for business logic function
                 (confirmUserServive:ConfirmUserServive') //Service depndency for business logic function
                 (aGroup:Group) 
                 (aUser:User)
                 :Boolean =

        let aStandardGroup = aGroup |> unwrapToStandardGroup
        let areBothFromSameTenant = aStandardGroup.TenantId = aUser.TenantId
        let userIsEnabled = aUser |> User.isEnabled

        match areBothFromSameTenant with
        | true -> 

            match userIsEnabled with
            | true  ->

                

                let groupMemberToFind = aUser |> User.toUserGroupMember

                match groupMemberToFind with 
                | Ok gMToFind ->

                    let foundMembers = aStandardGroup.Members 
                                      |> List.filter (
                                          fun nextGM -> gMToFind.MemberId = nextGM.MemberId
                                      )

                    match foundMembers with 
                    |[] -> false
                    |[aGroupMember] -> 
                        
                        let userConfirmationResult = confirmUserServive aGroup aUser 


                        match userConfirmationResult with 
                        | true ->

                            let IsUserInNestedGroupResult = isUserInNestedGroupService  aGroup aUser


                            if IsUserInNestedGroupResult then 
                                true
                            else
                                false

                        | false -> false

                    |head::tail ->
                        false
                
                | Error error ->
                    false

            | false ->
                false

        | false ->
             false



    let removeGroup (aGroupToRemoveFrom:Group) (aGroupToRemove:Group) : Result<Group, string> =


        if (areGroupsFromSameTenants aGroupToRemoveFrom aGroupToRemove) && ( aGroupToRemoveFrom |> isInternalGroup |> not ) then


            let rsFoundGroupMemberToRemoveFromCollection = result {

                let! groupMemberToRemove = aGroupToRemove |> toMemberOfTypeGroup'
                let unwrappedStandardGroup = aGroupToRemoveFrom  |> unwrapToStandardGroup

                let foundGroupMemberToRemoveFromCollection = unwrappedStandardGroup.Members
                                                             |> List.filter (fun g -> g.MemberId = groupMemberToRemove.MemberId)


                return foundGroupMemberToRemoveFromCollection
               
            }

            
            
            match rsFoundGroupMemberToRemoveFromCollection with 
            | Ok memberList -> 

                match memberList with
                | [] -> 
                    Error "Given group is not a member"
                | [l] -> 

                    let rsGroupMemberToRemove = result {

                        let! groupMemberToRemove = aGroupToRemove |> toMemberOfTypeGroup'

                        return groupMemberToRemove                    
                    }

                    let unwrappedStandardGroup = aGroupToRemoveFrom  |> unwrapToStandardGroup


                    match rsGroupMemberToRemove with 

                    | Ok groupMemberToRemove ->
                    
                        let updatedMembers = unwrappedStandardGroup.Members 
                                             |>List.filter (fun m -> m.MemberId = groupMemberToRemove.MemberId)

                        let newStGroup = { unwrappedStandardGroup with Members = updatedMembers }

                        Ok (Standard newStGroup)


                    | Error error ->
                        Error error

                | head::tail -> Error "Unconsostent state"


            | Error error 
                -> Error error 

        else

            let msg = sprintf "Wrong tenant consistency"

            Error msg

            








    let removeUser (aGroupToRemoveFrom:Group) (aUserToRemove:User) : Result<Group, string> =


        if (areGroupAndUserFromSameTenants aGroupToRemoveFrom aUserToRemove) && ( aGroupToRemoveFrom |> isInternalGroup |> not ) then


            let rsFoundGroupMemberToRemoveFromCollection = result {

                let! groupMemberToRemove = aUserToRemove |> toMemberOfTypeUser'
                let unwrappedStandardGroup = aGroupToRemoveFrom  |> unwrapToStandardGroup

                let foundGroupMemberToRemoveFromCollection = unwrappedStandardGroup.Members
                                                             |> List.filter (fun g -> g.MemberId = groupMemberToRemove.MemberId)


                return foundGroupMemberToRemoveFromCollection
               
            }

            
            
            match rsFoundGroupMemberToRemoveFromCollection with 
            | Ok memberList -> 

                match memberList with
                | [] -> 
                    Error "Given user is not a member"
                | [l] -> 

                    let rsGroupMemberToRemove = result {

                        let! groupMemberToRemove = aUserToRemove |> toMemberOfTypeUser'

                        return groupMemberToRemove                    
                    }

                    let unwrappedStandardGroup = aGroupToRemoveFrom  |> unwrapToStandardGroup


                    match rsGroupMemberToRemove with 

                    | Ok groupMemberToRemove ->
                    
                        let updatedMembers = unwrappedStandardGroup.Members 
                                             |>List.filter (fun m -> m.MemberId = groupMemberToRemove.MemberId)

                        let newStGroup = { unwrappedStandardGroup with Members = updatedMembers }

                        Ok (Standard newStGroup)


                    | Error error ->
                        Error error

                | head::tail -> Error "Unconsostent state"


            | Error error 
                -> Error error 

        else

            let msg = sprintf "Wrong tenant consistency"

            Error msg


            

























module Role = 



    open ServiceInterfaces
    open Group






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


            let internalGroup = Internal {
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











    let assignGroup (isGroupMemberService:IsGroupMemberService) (aRole:Role) (aGroup:Group) =

        let unwrappedGroup = aGroup |> unwrapToStandardGroup

        let areBothFromSameTenants = (aRole.TenantId = unwrappedGroup.TenantId)
        let doesRoleSupportNesting = (aRole.SupportNesting = SupportNestingStatus.Support)

        

        match areBothFromSameTenants with 
        | true -> 
            
            match doesRoleSupportNesting with  
            | true -> 


            
                Ok ( addGroupToGroup aRole.InternalGroup aGroup isGroupMemberService )



            | false -> 
                
                Error "Wrong tenant consistency"

        | false -> 
        
            
            Error "Wrong tenant consistency"








    let assignUser (aRole:Role) (aUser:User) =

        let areBothFromSameTenants = (aRole.TenantId = aUser.TenantId)

        match areBothFromSameTenants with 
        | true ->

            let rsGroupMember = result {
                

                
                let! userToGroupMember = aUser |> User.toUserGroupMember

                let roleInternalGroup = aRole.InternalGroup |> unwrapToStandardGroup  
                 
                let foundGrouMember = roleInternalGroup.Members
                                      |> List.tryFind (fun gm -> gm.MemberId = userToGroupMember.MemberId)

    

                return foundGrouMember, userToGroupMember
            }


            match rsGroupMember with 
            
            | Ok optionGmAndUser->


                match optionGmAndUser with
                | (Some fm, gmToAdd) -> 
                
                    Error "User already playing the role"

                | (None, gmToAdd) ->




                    let roleInternalStandardGroup = aRole.InternalGroup |> unwrapToStandardGroup  
                    let newStandarGroup = {roleInternalStandardGroup with Members = [gmToAdd]@roleInternalStandardGroup.Members}
                    let newRoleInternalGroup = Internal newStandarGroup
                    let newInternalRoleGroup = {aRole with InternalGroup = newRoleInternalGroup}

                    Ok newInternalRoleGroup

                           


            | Error error ->  
                Error error
                


            
        | false -> 
            Error "Wrond tenant consistency"



 







    let isInRole (isUserInNestedGroup:IsUserInNestedGroupService') (confirmUserServive:ConfirmUserServive') (aRole:Role) (aUser:User)  =

        let roleInternalGroup = aRole.InternalGroup
        isMember isUserInNestedGroup confirmUserServive roleInternalGroup aUser


        


