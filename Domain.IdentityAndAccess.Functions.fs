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
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Tenant

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Group





/// To be moved to common the namespace

let preppend  firstR restR = 
    match firstR, restR with
    | Ok first, Ok rest -> Ok (first::rest)  
    | Error error1, Ok _ -> Error error1
    | Ok _, Error error2 -> Error error2  
    | Error error1, Error _ -> Error error1

let ResultOfSequenceTemp aListOfResults =
    let initialValue = Ok List.empty
    List.foldBack preppend aListOfResults initialValue
///

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





    let provisionGroup (aTenant : Tenant.Tenant)(aGroupName :GroupName) (aGroupDescription: GroupDescription) : Result<Group.Group, string> =

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







    let activateTenant (aTenant : Tenant) (aReason : Reason) : Result<Tenant*Reason, string> =
     
            match aTenant.ActivationStatus with  
            | Deactivated ->
                Ok ({ aTenant with ActivationStatus = ActivationStatus.Activated }, aReason)
            | Activated -> 
                Error "Tenant already has its activation status set to Activated" 






    let deactivateTenant (aTenant : Tenant) (aReason : Reason) : Result<Tenant*Reason, string> =
        
        match aTenant.ActivationStatus with  
        | Deactivated ->
            Error "Tenant already has its activation status set to Deactivated"
        | Activated -> 
            Ok ({ aTenant with ActivationStatus = Deactivated }, aReason) 























module User = 

    let create id tenantId first middle last email address primaryPhone secondaryPhone username password = 
        
        let userConstruct = result {

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

        userConstruct






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





    let addGroupToGroup (aGroupToAddTo:Group.Group)(aGroupToAdd:Group.Group)(isGroupMemberService: IsGroupMemberService) : Result<Group.Group*GroupMember, string> =

        let unwrappedGroupToAddTo = match aGroupToAddTo with 
                                        | Standard aStandardGroup -> aStandardGroup
                                        | Internal anInternalGroup -> anInternalGroup
        let unwrappedGroupToAdd = match aGroupToAdd with 
                                        | Standard aStandardGroup -> aStandardGroup
                                        | Internal anInternalGroup -> anInternalGroup                                        

        let doBothGroupsHaveSameTenant = (unwrappedGroupToAddTo.TenantId = unwrappedGroupToAdd.TenantId)
        let isNotTheSameGroup  = not (unwrappedGroupToAddTo.GroupId = unwrappedGroupToAdd.GroupId)
        let toGroupGroupMember = toMemberOfTypeGroup GroupGroupMember
        let isGroupMemberService' = isGroupMemberService aGroupToAddTo 
        let isGroupToAddToRoleNotInterNalGroup =  aGroupToAddTo |> isInternalGroup |> not

        match doBothGroupsHaveSameTenant && isNotTheSameGroup && isGroupToAddToRoleNotInterNalGroup with 
        | true -> 
            let rsGrouMember = result {
                let! aMemberToAdd = aGroupToAdd |> toGroupGroupMember                                   
                let isTheGroupMemberToAddAlreadyAMember = aMemberToAdd |> isGroupMemberService' 
                return isTheGroupMemberToAddAlreadyAMember
                }

            match rsGrouMember with 
            | Ok isAlreadyGroupMember -> 

                printfn "RESULT isAlreadyGroupMember for aGroupToAddTo ==== %A" aGroupToAddTo
                printfn "RESULT isAlreadyGroupMember ==== %A" isAlreadyGroupMember


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
                        //let group = {aStandardGroupToAdd with Members = [groupMember]@newMembers}
                        Ok (aGroupToAddTo, groupMember)
                    | Error error ->
                        Error error
                else
                     Error "Group already a member"

            | Error isNotGoupMemberYet  -> 
                 Error isNotGoupMemberYet
           

        | false -> 
            let msg = sprintf "Wrong tenant consistency"
            Error msg

        




    
    let addUserToGroup (aGroupToAddTo:Group) (aUserToAdd:User) : Result<Group*GroupMember, string>   =


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

                        Ok (Standard {aStandardGroupToAdd with Members = aStandardGroupToAdd.Members@[grouMember]}, grouMember)

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


        










module Dto =  

    
    type TenantId = string

    type GroupId = string

    type Reason = {
        Description : string
        }
    
    ///User related types
    /// 
    /// 

    type FullName = {
        First: string
        Middle: string 
        Last: string
        }

    type EnablementStatus = 
        | Enabled = 1
        | Disabled = 0

    type Enablement = {
        EnablementStatus: EnablementStatus
        StartDate: DateTime
        EndDate: DateTime
        }

    type ContactInformation = {
        Email : string
        Address: string
        PrimaryTel: string
        SecondaryTel: string
        }      

    type ContactInformationChanged = 
        DomainEvent<ContactInformationChangedEventData>
    and ContactInformationChangedEventData = {
        EventVersion: int
        OccurredOn : DateTime
        TenantId: string
        Username: string
        FirstName: string
        LastName: string
        ContactInformation : string
        }

    type Person =  {
        Contact: ContactInformation
        Name: FullName 
        Tenant: TenantId
        User: UserId
        }

    type PersonalNameChanged = 
        DomainEvent<PersonalNameChangedEventData>
    and PersonalNameChangedEventData = {
        EventVersion: int
        OccurredOn : DateTime
        TenantId: string
        Username: string
        FirstName: string
        LastName: string
        }

    type User = {
        UserId : string
        TenantId : string
        Username : string
        Password : string
        Enablement : Enablement
        Person : Person
        }

    type RoleDescriptor = {
        RoleId: string
        TenantId: string
        Name: string
        }

    type UserDescriptor = {
        UserDescriptorId: string
        TenantId: string
        Username: string
        Email: string
        Roles: RoleDescriptor list 
        }

    type UserEnablementChanged =
            DomainEvent<UserEnablementChangedEventData>
    and UserEnablementChangedEventData = {
        EventVersion: int
        OccurredOn : DateTime
        TenantId: string
        Username: string
        }

    type UserPasswordChanged = 
            DomainEvent<UserPasswordChangedEventData>
    and UserPasswordChangedEventData = {
        EventVersion: int
        OccurredOn : DateTime
        TenantId: string
        Username: string
        }

    type UserRegisteredChanged = 
        DomainEvent<UserRegisteredChangedEventData>
    and UserRegisteredChangedEventData = {
        EventVersion: int
        OccurredOn : DateTime
        TenantId: string
        Username: string
        FirstName: string
        LastName: string
        }
    







    ///Group related types
    /// 
    ///
     
    type GroupMemberType = 
        | UserGroupMember = 1
        | GroupGroupMember = 2

    type GroupMember = {
        MemberId: string
        TenantId: string
        Name: string
        Type: GroupMemberType
        }

    type StandardGroup = {
        GroupId: string
        TenantId: string
        Name: string
        Description: string
        Members: GroupMember list
        }


    type StandardGroupCreated = {
        GroupId: string
        TenantId: string
        Group : StandardGroup
        }


    type GroupCreated = 
        | Standard of StandardGroupCreated
        | Internal of StandardGroupCreated


    type Group = 
        | Standard of StandardGroup
        | Internal of StandardGroup

    type UserAddedToGroup = {

        UserAdded : User
    }
    
    type GroupAddedToGroup = {

        GroupAdded : StandardGroup
    }

    type GroupGroupAdded = 
            DomainEvent<GroupGroupAddedEventData>
    and GroupGroupAddedEventData = {
        GroupId: string
        NestedGroupId: string
        TenantId: string
        }

    type GroupGroupRemoved = 
        DomainEvent<GroupGroupRemovedEventData>    
    and GroupGroupRemovedEventData = {
        GroupId: string
        RemovedGroupId: string
        TenantId: string
        }
    type GroupProvisioned = 
        DomainEvent<GroupProvisionedEventData>
    and GroupProvisionedEventData = {
        GroupId : string
        TenantId :string
        GroupName :string 
        }
    type GroupIdOrGroupMemberId = 
        | GroupId of GroupId
        | GroupMemberId of GroupMemberId

    type GroupMemberDto = {
        MemberId : string
        TenantId : string
        Name : string
        Type : string
        }


    ///Temporary types should there be here? let alone should they have been created?
    type GroupMemberDtoTemp = {
        MemberId : string
        TenantId : string
        Name : string
        Type : string
        }






    ///Role related types
    /// 
    ///  
        
    type SupportNestingStatus = 
        | Support = 1
        | Oppose = 2

    type Role = {
        RoleId: string
        TenantId: string
        Name: string
        Description: string
        SupportNesting: SupportNestingStatus
        InternalGroup: Group
        }
        
    type RoleProvisioned = 
        DomainEvent<RoleProvisionedEventData>
    and RoleProvisionedEventData = {
        RoleId : string
        TenantId : string
        }



    type GroupAssignedToRole = 
        DomainEvent<GroupAssignedToRoleEventData>
    and GroupAssignedToRoleEventData = {
        GroupId : string
        RoleId : string
        TenantId : string
        }

    type GroupUnAssignedToRole = 
        DomainEvent<GroupUnAssignedToRoleEventData>
    and GroupUnAssignedToRoleEventData = {
        GroupId : string
        RoleId : string
        TenantId : string
        }

    type UserAssignedToRole = 
        DomainEvent<UserAssignedToRoleEventData>
    and UserAssignedToRoleEventData = {
        GroupId : string
        RoleId : string
        TenantId : string
        }

    type UserUnAssignedToRole = 
        DomainEvent<UserUnAssignedToRoleEventData>
    and UserUnAssignedToRoleEventData = {
        GroupId : string
        RoleId : string
        TenantId : string
        }









    ///Tenant related types
    /// 
    /// 

    type ActivationStatus = 
        | Activated = 1
        | Deactivated = 0


    type ActivationStatusAndReason = {
        TenantId : string
        Status : ActivationStatus
        Reason : Reason
    }

    type RegistrationInvitation = {
        RegistrationInvitationId: string
        Description: string
        TenantId: string
        StartingOn: DateTime
        Until: DateTime
        }

    type OfferredRegistrationInvitation = {
        TenantId : string
        OfferredInvitation : RegistrationInvitation
    }

    type WithnrawnRegistrationInvitation = {
        TenantId : string
        WithdrawnInvitation : RegistrationInvitation
    }

    type Tenant = {
        TenantId: string
        Name: string
        Description: string 
        RegistrationInvitations: RegistrationInvitation list
        ActivationStatus : ActivationStatus
        }

    type TenantCreated = {
        TenantId: string
        Tenant : Tenant
        }

    type TenantActivationStatusActivated = 
        DomainEvent<TenantActivationStatusActivatedEventData>
    and TenantActivationStatusActivatedEventData = {
        TenantId : TenantId
        UserId : UserId
        }

    type TenantActivationStatusDiactivated = 
        DomainEvent<TenantActivationStatusDiactivatedEventData>
    and TenantActivationStatusDiactivatedEventData = {
        TenantId : TenantId
        UserId : UserId 
        }     

    type TenantAdministratorRegistered = 
        DomainEvent<TenantAdministratorRegisteredEventData>
    and TenantAdministratorRegisteredEventData = {
        TenantId : string
        FullName : string
        EmailAddress : string
        TenantName : string
        temporaryPassword : string
        Username : string
        }

    type TenantProvisioned =        
        DomainEvent<TenantProvisionedEventData> 
    and TenantProvisionedEventData = {
        TenantId : string
        }

    type RegistrationInvitationDto = {
        RegistrationInvitationId : RegistrationInvitationId
        TenantId : TenantId
        Description : RegistrationInvitationDescription
        StartingOn: DateTime
        Until : DateTime
        }

    type RegistrationInvitationDtoTemp = {
        RegistrationInvitationId : string
        TenantId : string
        Description : string
        StartingOn: DateTime
        Until : DateTime
        }

    type InvitationDescriptor = {
        RegistrationInvitationId : string
        TenantId : string
        RegistrationInvitationDescription : string
        StartingOn : DateTime
        Until : DateTime
        }

    type Provision = (Tenant*User*Role*RegistrationInvitation list)











    module User =


        let fromDomain (aUser:IdentityAndAcccess.DomainTypes.User.User) : User =

            let enablement = match aUser.Enablement.EnablementStatus with 
                             | Enabled -> EnablementStatus.Enabled
                             | Disabled -> EnablementStatus.Enabled


            let enablement: Enablement = {
                EnablementStatus = enablement
                StartDate = aUser.Enablement.StartDate
                EndDate  = aUser.Enablement.EndDate
            }

            let contact : ContactInformation = {
                Email =  aUser.Person.Contact.Email |> EmailAddress.value
                Address = aUser.Person.Contact.Address |> PostalAddress.value
                PrimaryTel = aUser.Person.Contact.PrimaryTel |> Telephone.value
                SecondaryTel = aUser.Person.Contact.SecondaryTel |> Telephone.value
                }

            let name : FullName = {
                First = aUser.Person.Name.First |> FirstName.value
                Middle = aUser.Person.Name.Middle |> MiddleName.value
                Last = aUser.Person.Name.Last |> LastName.value
            } 

            let person : Person = {
                Contact = contact
                Name = name
                Tenant = aUser.Person.Tenant |> TenantId.value
                User = aUser.Person.User
                }

            let userDto = {
                UserId = aUser.UserId |> UserId.value
                TenantId = aUser.TenantId |> TenantId.value
                Username = aUser.Username  |> Username.value
                Password = aUser.Password  |> Password.value
                Enablement = enablement 
                Person = person
                }

            userDto
        let toDomain (aUserDto:User) : Result<IdentityAndAcccess.DomainTypes.User.User, string> =

            result {

                let! email = (aUserDto.Person.Contact.Email |> EmailAddress.create')
                let! address = aUserDto.Person.Contact.Address |> PostalAddress.create'
                let! primaryTel = aUserDto.Person.Contact.PrimaryTel |> Telephone.create'
                let! secondaryTel = aUserDto.Person.Contact.SecondaryTel |> Telephone.create'

                let status:IdentityAndAcccess.DomainTypes.User.EnablementStatus
                         = match aUserDto.Enablement.EnablementStatus with
                           | EnablementStatus.Enabled -> IdentityAndAcccess.DomainTypes.User.EnablementStatus.Enabled
                           | EnablementStatus.Disabled -> IdentityAndAcccess.DomainTypes.User.EnablementStatus.Disabled
                           | _ -> IdentityAndAcccess.DomainTypes.User.EnablementStatus.Disabled

                let enablement : IdentityAndAcccess.DomainTypes.User.Enablement = {
                    EnablementStatus = status
                    StartDate = aUserDto.Enablement.StartDate
                    EndDate  = aUserDto.Enablement.EndDate
                    }

                let contact : IdentityAndAcccess.DomainTypes.User.ContactInformation = {
                    Email =  email
                    Address = address
                    PrimaryTel = primaryTel
                    SecondaryTel = secondaryTel 
                    }


                let! first = aUserDto.Person.Name.First |> FirstName.create'
                let! middle = aUserDto.Person.Name.Middle |> MiddleName.create'
                let! last = aUserDto.Person.Name.Last |> LastName.create'


                let name : IdentityAndAcccess.DomainTypes.User.FullName = {
                    First = first
                    Middle = middle
                    Last = last
                }

                let! tenantId = aUserDto.TenantId |> TenantId.create'
                let! userId = aUserDto.UserId |> UserId.create' 
                let! username = aUserDto.Username |> Username.create'
                let! password = aUserDto.Password |> Password.create' 


                let person : IdentityAndAcccess.DomainTypes.User.Person = {
                    Contact = contact
                    Name = name
                    Tenant = tenantId
                    User = userId
                    }

                let user: IdentityAndAcccess.DomainTypes.User.User = {
                    UserId = userId
                    TenantId = tenantId
                    Username = username
                    Password = password
                    Enablement = enablement
                    Person = person
                    }
                
                return user

            }

    module GroupMember = 

        let fromDomain (aGroupMember:IdentityAndAcccess.DomainTypes.Group.GroupMember) : GroupMember = 

                let memberType:GroupMemberType = 
                    match aGroupMember.Type with 
                    | IdentityAndAcccess.DomainTypes.Group.GroupGroupMember -> GroupMemberType.GroupGroupMember
                    | IdentityAndAcccess.DomainTypes.Group.UserGroupMember -> GroupMemberType.UserGroupMember
                        
                let gmDto : GroupMember = {
                    MemberId = aGroupMember.MemberId |> GroupMemberId.value
                    TenantId = aGroupMember.TenantId |> TenantId.value  
                    Name = aGroupMember.Name |> GroupMemberName.value
                    Type = memberType
                    }

                gmDto
                //Helpers

        


    module Group =


        let fromDomain (aGroup:IdentityAndAcccess.DomainTypes.Group.Group) : Group =

            let toGoupMemberDto (aGroupMember:IdentityAndAcccess.DomainTypes.Group.GroupMember) : GroupMember = 
            
                let memberType:GroupMemberType = 
                        match aGroupMember.Type with 
                        | IdentityAndAcccess.DomainTypes.Group.GroupGroupMember -> GroupMemberType.GroupGroupMember
                        | IdentityAndAcccess.DomainTypes.Group.UserGroupMember -> GroupMemberType.UserGroupMember
                        
                let gmDto : GroupMember = {
                    MemberId = aGroupMember.MemberId |> GroupMemberId.value
                    TenantId = aGroupMember.TenantId |> TenantId.value  
                    Name = aGroupMember.Name |> GroupMemberName.value
                    Type = memberType
                    }

                gmDto

            let unwrappedGroup = match aGroup with 
                                 | IdentityAndAcccess.DomainTypes.Group.Standard s ->  s
                                   | IdentityAndAcccess.DomainTypes.Group.Internal i ->  i

            let toGroupMemberDtoList = unwrappedGroup.Members 
                                       |> List.map toGoupMemberDto
      
            let sg:StandardGroup = {
                GroupId = unwrappedGroup.GroupId |>  GroupId.value
                TenantId = unwrappedGroup.TenantId |>  TenantId.value
                Name = unwrappedGroup.Name |>  GroupName.value
                Description = unwrappedGroup.Description |>  GroupDescription.value
                Members = toGroupMemberDtoList
                }

            Standard sg
        let toDomain (aGrouDto:Group) : Result<IdentityAndAcccess.DomainTypes.Group.Group, string> =


            let fromGoupMemberDomain (aGroupMember:GroupMember) : Result<IdentityAndAcccess.DomainTypes.Group.GroupMember, string> = 

                result {

                    let memberType:IdentityAndAcccess.DomainTypes.Group.GroupMemberType = 
                        match aGroupMember.Type with 
                        | GroupMemberType.GroupGroupMember -> IdentityAndAcccess.DomainTypes.Group.GroupMemberType.GroupGroupMember
                        | GroupMemberType.UserGroupMember -> IdentityAndAcccess.DomainTypes.Group.GroupMemberType.UserGroupMember
                      
            

                    let! memberId =  aGroupMember.MemberId |> GroupMemberId.create'
                    let! tenantId =  aGroupMember.TenantId |> TenantId.create'
                    let! name =  aGroupMember.Name |> GroupMemberName.create'

                    let gm : IdentityAndAcccess.DomainTypes.Group.GroupMember  = {
                        MemberId = memberId
                        TenantId = tenantId 
                        Name = name
                        Type = memberType
                        }
                    return gm
                }
                

                

            result {
                let unwrapToStandardGroup = match aGrouDto with 
                                            | Standard s -> s
                                            | Internal i -> i
                let! groupId = unwrapToStandardGroup.GroupId |> GroupId.create'
                let! tenantId = unwrapToStandardGroup.TenantId |> TenantId.create'
                let! name = unwrapToStandardGroup.Name |> GroupName.create'
                let! description =  unwrapToStandardGroup.Description |>  GroupDescription.create'

                let! members = unwrapToStandardGroup.Members
                              |> List.map fromGoupMemberDomain
                              |> ResultOfSequenceTemp
                              
                let standardGroup: IdentityAndAcccess.DomainTypes.Group.StandardGroup = {
                    GroupId = groupId 
                    TenantId = tenantId
                    Name = name 
                    Description = description
                    Members = members
                    }

                let group = match aGrouDto with 
                            | Standard s ->  IdentityAndAcccess.DomainTypes.Group.Standard standardGroup
                            | Internal i -> IdentityAndAcccess.DomainTypes.Group.Internal standardGroup

                return group
                }

            





    module Role =


 
        let fromDomain (aRole:IdentityAndAcccess.DomainTypes.Role.Role) : Role =

           
            let unwrappedIntenalGroup = 
                match aRole.InternalGroup with 
                | IdentityAndAcccess.DomainTypes.Group.Standard s ->  s
                | IdentityAndAcccess.DomainTypes.Group.Internal i ->  i

            let toGroupMemberDtoList = unwrappedIntenalGroup.Members 
                                       |> List.map GroupMember.fromDomain
      
            let sg:StandardGroup = {
                GroupId = unwrappedIntenalGroup.GroupId |>  GroupId.value
                TenantId = unwrappedIntenalGroup.TenantId |>  TenantId.value
                Name = unwrappedIntenalGroup.Name |>  GroupName.value
                Description = unwrappedIntenalGroup.Description |>  GroupDescription.value
                Members = toGroupMemberDtoList
                }

            let roleInternalGroup = Internal sg

            let supportNest = match aRole.SupportNesting with  
                              | IdentityAndAcccess.DomainTypes.Role.SupportNestingStatus.Support -> SupportNestingStatus.Support 
                              | IdentityAndAcccess.DomainTypes.Role.SupportNestingStatus.Oppose -> SupportNestingStatus.Oppose


            let role:Role = {
                RoleId =  aRole.RoleId |> RoleId.value
                TenantId = aRole.TenantId |>  TenantId.value
                Name = aRole.Name |>  RoleName.value
                Description = aRole.Description |>  RoleDescription.value
                SupportNesting = supportNest
                InternalGroup = roleInternalGroup
                }

            role
        let toDomain (aRoleDto:Role) : Result<IdentityAndAcccess.DomainTypes.Role.Role, string> =


            let fromGoupMemberDomain (aGroupMember:GroupMember) : Result<IdentityAndAcccess.DomainTypes.Group.GroupMember, string> = 

                result {

                    let memberType:IdentityAndAcccess.DomainTypes.Group.GroupMemberType = 
                        match aGroupMember.Type with 
                        | GroupMemberType.GroupGroupMember -> IdentityAndAcccess.DomainTypes.Group.GroupMemberType.GroupGroupMember
                        | GroupMemberType.UserGroupMember -> IdentityAndAcccess.DomainTypes.Group.GroupMemberType.UserGroupMember
            

                    let! memberId =  aGroupMember.MemberId |> GroupMemberId.create'
                    let! tenantId =  aGroupMember.TenantId |> TenantId.create'
                    let! name =  aGroupMember.Name |> GroupMemberName.create'

                    let gm : IdentityAndAcccess.DomainTypes.Group.GroupMember  = {
                        MemberId = memberId
                        TenantId = tenantId 
                        Name = name
                        Type = memberType
                        }
                    return gm
                }
                

                

            result {
                let unwrapToStandardGroup = match aRoleDto.InternalGroup with 
                                            | Standard s -> s
                                            | Internal i -> i


                let! roleId = aRoleDto.RoleId |> RoleId.create'
                let! roleTenantId = aRoleDto.TenantId |> TenantId.create'
                let! roleName = aRoleDto.Name |> RoleName.create'
                let! roleDescription = aRoleDto.Description |> RoleDescription.create'
                let supportNest  = match aRoleDto.SupportNesting  with  
                                   | SupportNestingStatus.Support -> IdentityAndAcccess.DomainTypes.Role.SupportNestingStatus.Support
                                   | SupportNestingStatus.Oppose -> IdentityAndAcccess.DomainTypes.Role.SupportNestingStatus.Oppose

                                     
                let! groupId = unwrapToStandardGroup.GroupId |> GroupId.create'
                let! tenantId = unwrapToStandardGroup.TenantId |> TenantId.create'
                let! name = unwrapToStandardGroup.Name |> GroupName.create'
                let! description =  unwrapToStandardGroup.Description |>  GroupDescription.create'




                let! members = unwrapToStandardGroup.Members
                              |> List.map fromGoupMemberDomain
                              |> ResultOfSequenceTemp
                              
                let standardInternalGroup: IdentityAndAcccess.DomainTypes.Group.StandardGroup = {
                    GroupId = groupId 
                    TenantId = tenantId
                    Name = name 
                    Description = description
                    Members = members
                    }

                let role : IdentityAndAcccess.DomainTypes.Role.Role = {
                    RoleId =  roleId
                    TenantId = roleTenantId
                    Name =roleName
                    Description = roleDescription
                    SupportNesting = supportNest
                    InternalGroup =  IdentityAndAcccess.DomainTypes.Group.Internal standardInternalGroup
                    }

                
                return role
                }

            




    module RegistrationInvitation = 


          let fromDomain (aRegistrationInvitation:Tenant.RegistrationInvitation) : RegistrationInvitation = 
            
            let regInDto:RegistrationInvitation = {

                RegistrationInvitationId = aRegistrationInvitation.RegistrationInvitationId |> RegistrationInvitationId.value
                Description = aRegistrationInvitation.Description |> RegistrationInvitationDescription.value
                TenantId = aRegistrationInvitation.TenantId |> TenantId.value
                StartingOn =  aRegistrationInvitation.StartingOn
                Until = aRegistrationInvitation.Until
            }
            regInDto
            







      
    module Tenant =


        let fromDomain (aTenant:IdentityAndAcccess.DomainTypes.Tenant.Tenant) : Tenant =

            let toRegistrationIntationDto (aRegistrationInvitation:IdentityAndAcccess.DomainTypes.Tenant.RegistrationInvitation) : RegistrationInvitation = 
                let rgDto : RegistrationInvitation = {
                    RegistrationInvitationId = aRegistrationInvitation.RegistrationInvitationId |> RegistrationInvitationId.value
                    Description = aRegistrationInvitation.Description |> RegistrationInvitationDescription.value
                    TenantId = aRegistrationInvitation.TenantId |> TenantId.value
                    StartingOn = aRegistrationInvitation.StartingOn
                    Until = aRegistrationInvitation.Until
                    }

                rgDto

            let status = match aTenant.ActivationStatus with 
                            | IdentityAndAcccess.DomainTypes.Tenant.ActivationStatus.Activated  ->  ActivationStatus.Activated 
                            | IdentityAndAcccess.DomainTypes.Tenant.ActivationStatus.Deactivated  ->  ActivationStatus.Deactivated 

      
            let tenant:Tenant = {
                TenantId = aTenant.TenantId |> TenantId.value
                Name = aTenant.Name |> TenantName.value
                Description = aTenant.Description |> TenantDescription.value
                RegistrationInvitations = aTenant.RegistrationInvitations |> List.map toRegistrationIntationDto
                ActivationStatus = status
                }
               

            tenant
            
        let toDomain (aTenantDto:Tenant) : Result<IdentityAndAcccess.DomainTypes.Tenant.Tenant, string> =


            let fromtoRegistrationIntationDomain (aRegistrationInvitation:RegistrationInvitation) : Result<IdentityAndAcccess.DomainTypes.Tenant.RegistrationInvitation, string> = 

                result {
                    let! regInvId = aRegistrationInvitation.RegistrationInvitationId |> RegistrationInvitationId.create'
                    let! desc =  aRegistrationInvitation.Description |> RegistrationInvitationDescription.create'
                    let! tenantId =   aRegistrationInvitation.TenantId |> TenantId.create'

                    let regInvTDomain : IdentityAndAcccess.DomainTypes.Tenant.RegistrationInvitation  = {
                        RegistrationInvitationId = regInvId
                        Description = desc 
                        TenantId = tenantId
                        StartingOn = aRegistrationInvitation.StartingOn
                        Until = aRegistrationInvitation.Until
                        }

                    return regInvTDomain
                } 

            result {
     
                let! id = aTenantDto.TenantId |> TenantId.create'
                let! name = aTenantDto.Name |> TenantName.create'
                let! description = aTenantDto.Description |> TenantDescription.create'
                let! id = aTenantDto.TenantId |> TenantId.create'
                let! fromtoRegistrationIntationDomainList = aTenantDto.RegistrationInvitations |> List.map fromtoRegistrationIntationDomain |> ResultOfSequenceTemp 

                let status = match aTenantDto.ActivationStatus with 
                              | ActivationStatus.Activated -> IdentityAndAcccess.DomainTypes.Tenant.ActivationStatus.Activated
                              | ActivationStatus.Deactivated -> IdentityAndAcccess.DomainTypes.Tenant.ActivationStatus.Deactivated

                let tenant : IdentityAndAcccess.DomainTypes.Tenant.Tenant = {
                    TenantId =  id
                    Name = name
                    Description = description
                    RegistrationInvitations = fromtoRegistrationIntationDomainList                     
                    ActivationStatus = status
                    }

                
                return tenant
                }

        










    module Other = 




        type Reason = {
            Description : string
            }





    ///Should be a common domain type 
    type DateTimeSpan = private {
        Start: DateTime
        End: DateTime
    } 












    //