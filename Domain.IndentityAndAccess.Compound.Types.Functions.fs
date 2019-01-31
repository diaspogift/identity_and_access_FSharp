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
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Tenant

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.Group








/// To be moved to common the namespace

let preppend  firstR restR = 
    match (firstR, restR) with
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
    objectId.ToString().ToUpper()



let isSameTenancy (tid1:TenantId) (tid2:TenantId)  = tid1 <> tid2


let passThroughLocal errorMessage f x y grpMember  = 
    if f x y then 
        Ok grpMember 
    else 
       Error errorMessage

let passThrough = passThroughLocal "Wrong tenant consistency!" 






type IsGroupMemberService =  Group -> Group  -> Result<Boolean,string>




let rec remove i l =
    match (i, l) with
    | 0, x::xs -> xs
    | i, x::xs -> x::remove (i - 1) xs
    | i, [] -> failwith "index out of range"

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
                -> Group  
                -> Result<Boolean,string>

    type IsGroupMemberServiceWithBakedGetGroupMemberByIdDependency = 
            Group 
                -> GroupMember   
                -> Boolean


    
    type IsUserInNestedGroupService = 
            Group 
                -> GroupMember 
                -> (GroupMember list * Boolean)




    type ConfirmUserServive = 
             Group 
                -> GroupMember 
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
                ->  Result<TenantProvision,string>






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
        Role
            -> User
            -> Boolean
            

    type AuthenticationService = 
            PasswordsMatcherService
                -> IsUserInRoleService
                -> User
                -> Tenant 
                -> Password 
                -> Result<UserDescriptor,string>






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
        | Deactivated -> false
        




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
                    MemberIn = []
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
                        MemberIn = []
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
        | ActivationStatus.Activated -> 
            aTenant.RegistrationInvitations
            |> List.filter RegistrationInvitations.isAvailableWithBakedDateTimeParam
        | Deactivated -> []






    let getAllUnAvailableRegistrationInvitation(aTenant:Tenant) : RegistrationInvitation list =

        match aTenant.ActivationStatus with 
        | ActivationStatus.Activated -> 
            aTenant.RegistrationInvitations
            |> List.filter RegistrationInvitations.isNotAvailableWithBakedDateTimeParam
        | Deactivated -> []



            


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
                |> List.filter 
                    (fun nextRegistrationInvitation -> 
                        nextRegistrationInvitation.RegistrationInvitationId = aRegistrationInvitationId
                        )
                |> List.first

            match concernedRegistrationInvitation with 
            | Some invitation -> 

                let remainingRegistrationInvitations = 
                    aTenant.RegistrationInvitations 
                    |> List.filter 
                        (fun nextRegistrationInvitation -> 
                            nextRegistrationInvitation.RegistrationInvitationId = aRegistrationInvitationId
                            ) 
                let newInvitation = {invitation with StartingOn = aStartDate; Until = anEndDate}
                    
                Ok {aTenant with RegistrationInvitations = remainingRegistrationInvitations@[newInvitation]}
            | None   -> 
                let msg = sprintf "Registration invitation with id %A not found" aRegistrationInvitationId
                Error msg
         | Deactivated -> 
               let error = sprintf "Tenant is deactivated"
               Error error






    let registerUserForTenant (aTenant:Tenant) (anInvitationId:RegistrationInvitationId) (aUSerName:Username) (aPassword:Password)
        (anEnablement:Enablement) (anEmailAddress:EmailAddress) (aPostalAddress:PostalAddress) (aPrimaryTel:Telephone) 
        (aSecondaryTel:Telephone) (aFirstName:FirstName) (aMiddleNAme:MiddleName)(aLastName:LastName) :Result<User,string> =

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
                    let user:User = {
                        UserId = userId 
                        TenantId = aTenant.TenantId 
                        Username = aUSerName 
                        Password = aPassword 
                        Enablement = anEnablement
                        Person = person
                        }
                    return user
                    }
                match rsCreateUser with 
                | Ok user -> 
                    Ok user
                | Error error -> 
                    Error error
            | false ->
                let msg = sprintf "Registration expired/notavailable "
                Error msg   
        | Deactivated ->
            let msg = sprintf "Tenant deactivated"
            Error msg





    let withdrawRegistrationInvitation (aTenant:Tenant) (invitation:RegistrationInvitationId) :Result<Tenant * RegistrationInvitation, string> =

        // printfn "TENANT: === "
        // printfn "TENANT: === "
        // printfn "TENANT: === %A" aTenant
        // printfn "INVITATION TO REMOVE: === " 
        // printfn "INVITATION TO REMOVE: === " 
        // printfn "INVITATION TO REMOVE: === %A" invitation


        let optionalInvitationToWithdraw = 
            aTenant.RegistrationInvitations 
            |> List.filter (fun nextInvitation -> nextInvitation.RegistrationInvitationId = invitation )
            |> List.first

        // printfn "INVITATION TO REMOVE FOUND: === " 
        // printfn "INVITATION TO REMOVE FOUND: === " 
        // printfn "INVITATION TO REMOVE FOUND: === %A" optionalInvitationToWithdraw

        match optionalInvitationToWithdraw with 
        | Some reg -> 

            let otherInvitations = 
                aTenant.RegistrationInvitations 
                |> List.filter (fun nextInvitation -> not (nextInvitation.RegistrationInvitationId = invitation) )

            // printfn "otherInvitations: === " 
            // printfn "otherInvitations: === " 
            // printfn "otherInvitations: === %A" otherInvitations
                    
            Ok ({aTenant with RegistrationInvitations = otherInvitations }, reg)

        | None ->
            let msg = sprintf "No registration invitation found for identifier: %A" invitation
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

    


    let changePassWord aUser aCurrentGivenPassword aNewGivenPassword = 
        if aUser.Password <> aCurrentGivenPassword then
            Error "Current Password not confirmed"
        else
            Ok {aUser with Password = aNewGivenPassword} 







    let changePersonalContactInformation (aUser:User) (aContactInformation:ContactInformation) = 
        let personWithNewContact = {aUser.Person with Contact = aContactInformation}
        {aUser with Person = personWithNewContact}






    let changePersonalName (aUser:User) (aFullName:FullName) = 
        let personWithNewName = {aUser.Person with Name = aFullName}
        {aUser with Person = personWithNewName}






    let defineEnablement (aUser:User) anEnablement = 
        let userWithNewEnablement = {aUser with Enablement = anEnablement}
        userWithNewEnablement






    let isEnabled (aUser:User)  = 
        match aUser.Enablement.EnablementStatus with  
        | Enabled -> true
        | Disabled -> false






    let isDisabled (aUser:User)   = 
        match aUser.Enablement.EnablementStatus with  
        | Enabled -> false
        | Disabled -> true





    let toGroupMember (memberType:GroupMemberType) (aUser:User) : Result<GroupMember,string> =

        let rs = result {

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





















































module Group = 




 

    open ServiceInterfaces




    let areGroupsFromSameTenants (firstGroup:Group) (secondGroup:Group) = 
        let uF = unwrapToStandardGroup firstGroup
        let uS = unwrapToStandardGroup secondGroup

        uF.TenantId = uS.TenantId   

    
    let areGroupAndUserFromSameTenants (aGroup:Group) (aUser:User) = 
        let uG = unwrapToStandardGroup aGroup
        uG.TenantId = aUser.TenantId   


    let toMemberOfTypeGroupLocal (memberType:GroupMemberType) (aGroup:Group) =

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

    let toMemberOfTypeGroup =  toMemberOfTypeGroupLocal  GroupMemberType.GroupGroupMember
    let toMemberOfTypeUser' =  toMemberOfTypeUser  GroupMemberType.UserGroupMember


    let createFull (members : GroupMember list) (groupId:GroupId) (tenantId:TenantId) (name:GroupName) (description:GroupDescription)  = 
        
        let rsCreateGroup = result {

            return Standard {
               GroupId = groupId
               TenantId = tenantId
               Name = name
               Description = description
               Members = members
               MemberIn = []
            }
        }

        rsCreateGroup

        
  

    let isStandardGroup aGroup = 
        match aGroup with 
        | Standard g -> true
        | Internal g -> false   



    let isInternalGroup aGroup = 
        match aGroup with 
        | Standard g -> false
        | Internal g -> true   


    let checkNotSame (aGroup1:StandardGroup) (aGroup2:StandardGroup) = 
         aGroup1.GroupId <> aGroup2.GroupId
    
    let checkSameTenancy (aTenantId1:TenantId) (aTenantId2:TenantId) = 
         aTenantId1 = aTenantId2


    let addGroupToGroup (aGroupToAddTo:Group.Group)(aGroupToAdd:Group.Group)(isGroupMemberService: IsGroupMemberService) 
        : Result<Group.Group*Group.GroupMember*Group.Group*Group.GroupMember, string> =

        let unwrappedGroupToAddTo = aGroupToAddTo |> unwrapToStandardGroup 
        let unwrappedGroupToAdd = aGroupToAdd |> unwrapToStandardGroup
        printfn "----------------------------------------------------------------------------------------------------"
        printfn "aGroupToAddTo =====  %A "  aGroupToAddTo 
        printfn "----------------------------------------------------------------------------------------------------"
        
        printfn "----------------------------------------------------------------------------------------------------"
        printfn "aGroupToAdd =====  %A "  aGroupToAdd
        printfn "----------------------------------------------------------------------------------------------------"
        printfn "----------------------------------------------------------------------------------------------------"
        
        printfn ""

        printfn "----------------------------------------------------------------------------------------------------"

        printfn "----------------------------------------------------------------------------------------------------"
        printfn "unwrappedGroupToAddTo =  %A "  unwrappedGroupToAddTo 
        printfn "----------------------------------------------------------------------------------------------------"
        
        printfn "----------------------------------------------------------------------------------------------------"
        printfn "unwrappedGroupToAdd =  %A "  unwrappedGroupToAdd
        printfn "----------------------------------------------------------------------------------------------------"
        
                                                
        let doBothGroupsHaveSameTenant = (unwrappedGroupToAddTo.TenantId = unwrappedGroupToAdd.TenantId)
        let isNotTheSameGroup  = unwrappedGroupToAdd |> checkNotSame unwrappedGroupToAddTo   
        let isGroupToAddToRoleNotInterNalGroup =  aGroupToAddTo |> isInternalGroup |> not

        match doBothGroupsHaveSameTenant && isNotTheSameGroup && isGroupToAddToRoleNotInterNalGroup with 
        | true -> 
            let rsGroupMember = result {
                let! isTheGroupMemberToAddAlreadyAMember = aGroupToAdd |> isGroupMemberService aGroupToAddTo
                return isTheGroupMemberToAddAlreadyAMember
                }
            match rsGroupMember with 
            | Ok isAlreadyGroupMember -> 
                printfn "RESULT isAlreadyGroupMember ==== %A" isAlreadyGroupMember
                if not isAlreadyGroupMember then   
                    let rsIsGrouMemberToAdd = result {
                        let! aMemberToAdd = aGroupToAdd |> toMemberOfTypeGroup
                        let! aMemberToAddTo = aGroupToAddTo |> toMemberOfTypeGroup
                        return (aMemberToAdd, aMemberToAddTo)
                        }
                    match rsIsGrouMemberToAdd with 
                    | Ok (groupMember, groupMemberIn) -> 
                        let oneListGrpMember = groupMember |> List.singleton
                        let oneListGrpMemberIn = groupMemberIn |> List.singleton
                        let newStandardGroupToAddTo = {unwrappedGroupToAddTo with Members = unwrappedGroupToAddTo.Members @ oneListGrpMember }
                        let newStandardGroupToAdd = {unwrappedGroupToAdd with MemberIn = unwrappedGroupToAdd.MemberIn @ oneListGrpMemberIn }
                        match aGroupToAddTo with  
                        | Group.Standard _ -> 
                            
                            Ok (Standard newStandardGroupToAddTo, groupMember, Standard newStandardGroupToAdd, groupMemberIn)
                        | Group.Internal _ -> 
                            Ok (Internal newStandardGroupToAddTo, groupMember, Internal newStandardGroupToAdd, groupMemberIn)
                    | Error error ->
                        Error error
                else
                    Error "Group already a member"
            | Error error  -> 
                Error error
        | false -> 
            let msg = sprintf "Wrong tenant consistency"
            Error msg

        

    let addUserToGroup (aGroupToAddTo:Group) (aUserToAdd:User) : Result<Group*GroupMember, string>   =


        let aStandardGroupToAddTo =  aGroupToAddTo |> DomainHelpers.unwrapToStandardGroup
        let areFromSameTenant = aUserToAdd.TenantId |> checkSameTenancy aStandardGroupToAddTo.TenantId 
        let isUserEnabled = aUserToAdd |> User.isEnabled
        let isGroupToAddToRoleNotInterNalGroup = aGroupToAddTo |> isInternalGroup |> not        
   
        match areFromSameTenant && isUserEnabled && isGroupToAddToRoleNotInterNalGroup with  
        | true -> 
            let isUseGroupMember = result {
                let! groupMemberToAdd = aUserToAdd |> toMemberOfTypeUser'
                let searchGivenMemberInGroupMembers = 
                    aStandardGroupToAddTo.Members
                    |> List.filter (
                      fun next -> 
                        groupMemberToAdd.MemberId = next.MemberId
                    )
                let isUseGroupMember = 
                    match searchGivenMemberInGroupMembers with 
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
                    | Ok grouMember -> Ok (Standard {aStandardGroupToAddTo with Members = aStandardGroupToAddTo.Members@[grouMember]}, grouMember)
                    | Error error -> Error error
                | true ->
                    let msg = sprintf "Already group member" 
                    Error msg
            | Error _ -> 
                let msg = sprintf "Wrong tenant consistency 2" 
                Error msg
        | false ->
            let msg = sprintf "Wrong tenant consistency or disabled user account" 
            Error msg
  

            

    let isMember (isUserInNestedGroupService:IsUserInNestedGroupService) //Service depndency for business logic function
                 (confirmUserServive:ConfirmUserServive) //Service depndency for business logic function
                 (aGroup:Group) 
                 (aUserMember:GroupMember)
                 :(GroupMember list * Boolean) =

        let aStandardGroup = aGroup |> unwrapToStandardGroup
        let areBothFromSameTenant = aStandardGroup.TenantId = aUserMember.TenantId
        //let userIsEnabled = aUserMember |> User.isEnabled

        

        let foundMembers = aStandardGroup.Members |> List.filter (fun nextGM -> aUserMember = nextGM)

        match foundMembers with 
         |[foundMember] -> 
            match foundMember |> confirmUserServive aGroup  with 
            | true -> 
                ([], true)
            | false -> 
                aUserMember |> isUserInNestedGroupService  aGroup 
         |_::_ -> ([], false)
         |[] -> ([], false)
     




    let removeGroup (aGroupToRemoveFrom:Group) (aGroupToRemove:Group) : Result<Group, string> =


        if (areGroupsFromSameTenants aGroupToRemoveFrom aGroupToRemove) && ( aGroupToRemoveFrom |> isInternalGroup |> not ) then


            let rsFoundGroupMemberToRemoveFromCollection = result {

                let! groupMemberToRemove = aGroupToRemove |> toMemberOfTypeGroup
                let unwrappedStandardGroup = aGroupToRemoveFrom  |> unwrapToStandardGroup

                let foundGroupMemberToRemoveFromCollection = 
                    unwrappedStandardGroup.Members
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

                        let! groupMemberToRemove = aGroupToRemove |> toMemberOfTypeGroup

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
            | Error error -> Error error 

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


    let addToMemberIn(aGroupToAddMembersTo:Group) (aMemberToAdd:GroupMember) : Result<Group,string> =
        let unwrappedGroupToAddTo = 
            match aGroupToAddMembersTo with 
            | Group.Standard aStandardGroup -> aStandardGroup 
            | Group.Internal anInternalGroup -> anInternalGroup 
        let groupsThatIAmMemberIn = unwrappedGroupToAddTo.Members
        if not (groupsThatIAmMemberIn |> List.contains aMemberToAdd) then  
            let aMemberToAddList = aMemberToAdd |> List.singleton
            match aGroupToAddMembersTo with  
            | Group.Standard _ ->  
                Ok (Group.Standard {unwrappedGroupToAddTo with MemberIn = unwrappedGroupToAddTo.MemberIn @ aMemberToAddList})
            | Group.Internal _ ->  
                Ok (Group.Internal {unwrappedGroupToAddTo with MemberIn = unwrappedGroupToAddTo.MemberIn @ aMemberToAddList})
        else Error "Already present in the reference group that I am member in"
            
                                                
                                                           

    let removeFromMemberIn(aGroupToRemoveMemberFrom:Group) (aMemberToRemove:GroupMember) : Result<Group,string> =

        let unwrappedGroupToRemoveFrom = 
            match aGroupToRemoveMemberFrom with 
            | Group.Standard aStandardGroup -> aStandardGroup 
            | Group.Internal anInternalGroup -> anInternalGroup 
        let groupsThatIAmMemberIn = unwrappedGroupToRemoveFrom.Members
        if not (groupsThatIAmMemberIn |> List.contains aMemberToRemove) then  
          Error "Already present in the reference group that I am member in"  
        else
            let newListOGroupMemberRefs =
                unwrappedGroupToRemoveFrom.MemberIn 
                |> List.filter (fun aGroupIamMemberIn ->  aGroupIamMemberIn = aMemberToRemove)
            Ok (Group.Standard {unwrappedGroupToRemoveFrom with MemberIn = newListOGroupMemberRefs})
            
            
                                                























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
                MemberIn = []
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

        

            result {


                let sameTenancyChecked = passThrough  isSameTenancy aRole.TenantId  aUser.TenantId 
  
                 
                let! userToGroupMember = aUser |> User.toUserGroupMember
                let roleInternalGroup = aRole.InternalGroup |> unwrapToStandardGroup   
                
                     
                let! foundGroupMember = 
                     roleInternalGroup.Members 
                     |> List.tryFind (fun gm -> gm.MemberId = userToGroupMember.MemberId)
                     |> Result.ofOption "User already playing the role"
                     
                let! groupMember = foundGroupMember |> sameTenancyChecked

                let roleInternalStandardGroup = aRole.InternalGroup |> unwrapToStandardGroup  
                let newStandarGroup = {roleInternalStandardGroup with Members = [groupMember]@roleInternalStandardGroup.Members}
                let newRoleInternalGroup = Internal newStandarGroup
                let newInternalRoleGroup = {aRole with InternalGroup = newRoleInternalGroup}
               
                                        
                return (newInternalRoleGroup, userToGroupMember)

                }    



 







    let isInRole (isUserInNestedGroup:IsUserInNestedGroupService) (confirmUserServive:ConfirmUserServive) (aRole:Role) (aUserMember:GroupMember)  =

        let roleInternalGroup = aRole.InternalGroup
        isMember isUserInNestedGroup confirmUserServive roleInternalGroup aUserMember


        










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
        Event<ContactInformationChangedEventData>
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
        Event<PersonalNameChangedEventData>
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
            Event<UserEnablementChangedEventData>
    and UserEnablementChangedEventData = {
        EventVersion: int
        OccurredOn : DateTime
        TenantId: string
        Username: string
        }

    type UserPasswordChanged = 
            Event<UserPasswordChangedEventData>
    and UserPasswordChangedEventData = {
        EventVersion: int
        OccurredOn : DateTime
        TenantId: string
        Username: string
        }

    type UserRegisteredChanged = 
        Event<UserRegisteredChangedEventData>
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
        MemberIn : GroupMember list
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
            Event<GroupGroupAddedEventData>
    and GroupGroupAddedEventData = {
        GroupId: string
        NestedGroupId: string
        TenantId: string
        }

    type GroupGroupRemoved = 
        Event<GroupGroupRemovedEventData>    
    and GroupGroupRemovedEventData = {
        GroupId: string
        RemovedGroupId: string
        TenantId: string
        }
    type GroupProvisioned = 
        Event<GroupProvisionedEventData>
    and GroupProvisionedEventData = {
        GroupId : string
        TenantId :string
        GroupName :string 
        }
    type GroupIdOrGroupMemberId = 
        | GroupId of GroupId
        | GroupMemberId of GroupMemberId


    type MemberAddedToGroupEvent = { 
        GroupId : string
        TenantId : string
        MemberAdded : GroupMember
        }

    type MemberInAddedToGroupEvent = { 
        GroupId : string
        TenantId : string
        MemberInAdded : GroupMember
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
        Event<RoleProvisionedEventData>
    and RoleProvisionedEventData = {
        RoleId : string
        TenantId : string
        }



    type GroupAssignedToRole = 
        Event<GroupAssignedToRoleEventData>
    and GroupAssignedToRoleEventData = {
        GroupId : string
        RoleId : string
        TenantId : string
        }

    type GroupUnAssignedToRole = 
        Event<GroupUnAssignedToRoleEventData>
    and GroupUnAssignedToRoleEventData = {
        GroupId : string
        RoleId : string
        TenantId : string
        }

    type UserAssignedToRole = 
        Event<UserAssignedToRoleEventData>
    and UserAssignedToRoleEventData = {
        GroupId : string
        RoleId : string
        TenantId : string
        }

    type UserUnAssignedToRole = 
        Event<UserUnAssignedToRoleEventData>
    and UserUnAssignedToRoleEventData = {
        GroupId : string
        RoleId : string
        TenantId : string
        }


    type UserAssignedToRoleEvent = { 
        RoleId : string
        UserId : string
        AssignedUser : GroupMember
        }

    type UserUnAssignedFromRoleEvent = { 
        RoleId : string
        UserId : string
        AssignedUser : GroupMember
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
        Event<TenantActivationStatusActivatedEventData>
    and TenantActivationStatusActivatedEventData = {
        TenantId : TenantId
        UserId : UserId
        }

    type TenantActivationStatusDiactivated = 
        Event<TenantActivationStatusDiactivatedEventData>
    and TenantActivationStatusDiactivatedEventData = {
        TenantId : TenantId
        UserId : UserId 
        }     

    type TenantAdministratorRegistered = 
        Event<TenantAdministratorRegisteredEventData>
    and TenantAdministratorRegisteredEventData = {
        TenantId : string
        FullName : string
        EmailAddress : string
        TenantName : string
        temporaryPassword : string
        Username : string
        }

    type TenantProvisioned =        
        Event<TenantProvisionedEventData> 
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

    //type Provision = (Tenant*User*Role*RegistrationInvitation list)











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
        let private toGroupMember (memberType:GroupMemberType) (aUser:User) :GroupMember =
            let groupMember:GroupMember = {
                    MemberId = aUser.UserId
                    TenantId = aUser.TenantId
                    Name = aUser.Person.Name.First + aUser.Person.Name.Last
                    Type = memberType
                }
            groupMember
        let toUserGroupMember = toGroupMember GroupMemberType.UserGroupMember
         

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

            let toGroupMemberDtoList = unwrappedGroup.Members |> List.map toGoupMemberDto
            let toGroupMemberRefsDtoList = unwrappedGroup.MemberIn |> List.map toGoupMemberDto
           
      
            let sg:StandardGroup = {
                GroupId = unwrappedGroup.GroupId |>  GroupId.value
                TenantId = unwrappedGroup.TenantId |>  TenantId.value
                Name = unwrappedGroup.Name |>  GroupName.value
                Description = unwrappedGroup.Description |>  GroupDescription.value
                Members = toGroupMemberDtoList
                MemberIn = toGroupMemberRefsDtoList
                }

            Standard sg
        let toDomain (aGrouDto:Group) : Result<IdentityAndAcccess.DomainTypes.Group.Group, string> =


            let fromGoupMemberDomain (aGroupMember:GroupMember) : Result<IdentityAndAcccess.DomainTypes.Group.GroupMember, string> = 

                result {

                    let! memberType = 
                        match aGroupMember.Type with 
                        | GroupMemberType.GroupGroupMember -> Ok IdentityAndAcccess.DomainTypes.Group.GroupMemberType.GroupGroupMember
                        | GroupMemberType.UserGroupMember -> Ok IdentityAndAcccess.DomainTypes.Group.GroupMemberType.UserGroupMember
                        | _ -> Error "Unknown group member type"
                      
            

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
                let unwrapToStandardGroup = 
                    match aGrouDto with 
                    | Standard s -> s
                    | Internal i -> i
                let! groupId = unwrapToStandardGroup.GroupId |> GroupId.create'
                let! tenantId = unwrapToStandardGroup.TenantId |> TenantId.create'
                let! name = unwrapToStandardGroup.Name |> GroupName.create'
                let! description =  unwrapToStandardGroup.Description |>  GroupDescription.create'

                let! members = unwrapToStandardGroup.Members |> List.map fromGoupMemberDomain |> ResultOfSequenceTemp
                let! memberIns = unwrapToStandardGroup.MemberIn |> List.map fromGoupMemberDomain |> ResultOfSequenceTemp
                                          
                let standardGroup: IdentityAndAcccess.DomainTypes.Group.StandardGroup = {
                    GroupId = groupId 
                    TenantId = tenantId
                    Name = name 
                    Description = description
                    Members = members
                    MemberIn = memberIns
                    }

                let group = 
                    match aGrouDto with 
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

            let toGroupMemberDtoList = unwrappedIntenalGroup.Members |> List.map GroupMember.fromDomain
            let toGroupMemberRefsDtoList = unwrappedIntenalGroup.MemberIn |> List.map GroupMember.fromDomain

      
            let sg:StandardGroup = {
                GroupId = unwrappedIntenalGroup.GroupId |>  GroupId.value
                TenantId = unwrappedIntenalGroup.TenantId |>  TenantId.value
                Name = unwrappedIntenalGroup.Name |>  GroupName.value
                Description = unwrappedIntenalGroup.Description |>  GroupDescription.value
                Members = toGroupMemberDtoList
                MemberIn = toGroupMemberRefsDtoList
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

                    let! memberType = 
                        match aGroupMember.Type with 
                        | GroupMemberType.GroupGroupMember -> Ok IdentityAndAcccess.DomainTypes.Group.GroupMemberType.GroupGroupMember
                        | GroupMemberType.UserGroupMember -> Ok IdentityAndAcccess.DomainTypes.Group.GroupMemberType.UserGroupMember
                        | _ -> Error "Unknown group member type"            

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
                let! supportNest  = 
                        match aRoleDto.SupportNesting  with  
                        | SupportNestingStatus.Support -> Ok IdentityAndAcccess.DomainTypes.Role.SupportNestingStatus.Support
                        | SupportNestingStatus.Oppose -> Ok IdentityAndAcccess.DomainTypes.Role.SupportNestingStatus.Oppose
                        | _ -> Error "Unknown group member type"            

                                     
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
                    MemberIn = []
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
     
                let! name = aTenantDto.Name |> TenantName.create'
                let! description = aTenantDto.Description |> TenantDescription.create'
                let! id = aTenantDto.TenantId |> TenantId.create'
                let! fromtoRegistrationIntationDomainList = aTenantDto.RegistrationInvitations |> List.map fromtoRegistrationIntationDomain |> ResultOfSequenceTemp 

                let! status = 
                        match aTenantDto.ActivationStatus with 
                        | ActivationStatus.Activated -> Ok IdentityAndAcccess.DomainTypes.Tenant.ActivationStatus.Activated
                        | ActivationStatus.Deactivated -> Ok IdentityAndAcccess.DomainTypes.Tenant.ActivationStatus.Deactivated
                        | _ -> Error "Unknown group member type"            

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