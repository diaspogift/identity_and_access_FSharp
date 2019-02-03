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
open IdentityAndAcccess.DomainTypes.User
open System.Collections.Generic
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.CommonDomainTypes









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



let generateNoEscapeId () =   
    let objectId = ObjectId.GenerateNewId()
    objectId.ToString().ToUpper()



let isSameTenancy (tid1:TenantId) (tid2:TenantId)  = tid1 = tid2


let passThroughLocal errorMessage f x y grpMember  = 
    if f x y then 
        Ok grpMember 
    else 
       Error errorMessage

//let passThrough = passThroughLocal "Wrong tenant consistency!" 






let userNotPlayingRoleCheck (aRole:Role.Role) (aUser:User.User) :(Result<bool, string>) =

    let resultCheck = 
         aRole.UsersThatPlayMe 
         |> List.exists (fun nextGroupDesc -> nextGroupDesc.UserId = aUser.UserId)
         |> (fun rsCheck ->
             if rsCheck then Error "User already playing the role" 
             else Ok rsCheck
             ) 
    resultCheck

let passThroughLocal1 errorMessage f x y grpMember  = 
    match (f x y) with 
    | Ok true -> Error errorMessage
    | Ok false -> Ok grpMember 
    | Error error -> Error error
       

//let passThrough1 = passThroughLocal1 "User already playing the role !" 

//let memberPlaysRoleChecked = passThrough1  


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
            Group.Group
                -> User.User 
                -> (GroupDescriptor list * Boolean)




    type ConfirmUserServive = 
             Group.Group 
                -> User.User 
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

                let group:Group.Group = {
                    GroupId = groupId 
                    TenantId = aTenant.TenantId
                    Name = aGroupName
                    Description = aGroupDescription
                    UsersAddedToMe = []
                    GroupsAddedToMe = []
                    GroupsIamAddedTo = []
                    RolesIPlay = []
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


            if aTenant.ActivationStatus = ActivationStatus.Activated then


                let role = result {

                    let! roleId = generateNoEscapeId() |> RoleId.create' 

                    ///TODO MAKE THIS CONSTRUT PRIVATE
                    let role : Role = {
                        RoleId = roleId 
                        TenantId = aTenant.TenantId
                        Name = aRoleName
                        Description = aRoleDescription
                        SupportNesting = SupportNestingStatus.Support
                        GroupsThatPlayMe = []
                        UsersThatPlayMe = []
                        }
                    return role
                    }
                role

            else Error "Tenant activation status deactivated" 







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
                        RolesIPlay = []
                        GroupsIAmIn = []
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

        let optionalInvitationToWithdraw = 
            aTenant.RegistrationInvitations 
            |> List.filter (fun nextInvitation -> nextInvitation.RegistrationInvitationId = invitation )
            |> List.first


        match optionalInvitationToWithdraw with 
        | Some reg -> 

            let otherInvitations = 
                aTenant.RegistrationInvitations 
                |> List.filter (fun nextInvitation -> nextInvitation.RegistrationInvitationId <> invitation )
                    
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

    
    let toDescriptor (aUser:User.User) : UserDescriptor =
        {
            UserId = aUser.UserId
            TenantId = aUser.TenantId
            Username = aUser.Username
            Email = aUser.Person.Contact.Email
            FirstName = aUser.Person.Name.First
            LastName = aUser.Person.Name.Last

        }


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



    let toUserDesriptor (aUser:User): UserDescriptor =

        {
            UserId = aUser.UserId 
            TenantId = aUser.TenantId 
            Username = aUser.Username 
            Email = aUser.Person.Contact.Email 
            FirstName = aUser.Person.Name.First 
            LastName = aUser.Person.Name.Last 
        }





























         













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
        
        
        









































































module Group = 




 

    open ServiceInterfaces



    let toDescriptor (aGroup:Group.Group) = 
        let gd:GroupDescriptor = {
            Id = aGroup.GroupId 
            TenantId = aGroup.TenantId
            Name = aGroup.Name
            ShortDesc = aGroup.Description 
        }
        gd


   
    let createFull (groupId:GroupId) (tenantId:TenantId) (name:GroupName) (description:GroupDescription)  = 
        
        let group: Group.Group = {
           GroupId = groupId
           TenantId = tenantId
           Name = name
           Description = description
           UsersAddedToMe = []
           GroupsAddedToMe = []
           GroupsIamAddedTo = []
           RolesIPlay = []
        }
        group
        



    let checkNotSame (aGroup1:Group.Group) (aGroup2:Group.Group) = 
         aGroup1 <> aGroup2
    
    let checkSameTenancy (aTenantId1:TenantId) (aTenantId2:TenantId) = 
         aTenantId1 = aTenantId2


    



    let addUserToGroup (aGroupToAddTo:Group.Group) (aUserToAdd:User.User) : Result<Group.Group*UserDescriptor, string>   =


       if aGroupToAddTo.TenantId |> checkSameTenancy aUserToAdd.TenantId then 
           
            let userToAddDesc = aUserToAdd |> User.toDescriptor

            if aGroupToAddTo.UsersAddedToMe |> List.exists (fun ud -> ud = userToAddDesc) then 
                Error "User already in the group"

            else 
                let aNewGroupToAddTo:Group.Group = {aGroupToAddTo with UsersAddedToMe = aGroupToAddTo.UsersAddedToMe @ [userToAddDesc]}
                Ok (aNewGroupToAddTo, userToAddDesc)
       else 
            Error "Wrong tenant consistency"

            

            

    let removeUser (aGroupToRemoveFrom:Group) (aUserToRemove:User) : Result<Group, string> =


  
        if (aGroupToRemoveFrom.TenantId |> isSameTenancy  aUserToRemove.TenantId ) then

            let aUserToRemoveDesc = aUserToRemove |> User.toDescriptor
            let newUserListCheck = aGroupToRemoveFrom.UsersAddedToMe |> List.filter (fun ud -> ud = aUserToRemoveDesc) 
            let newUserList = aGroupToRemoveFrom.UsersAddedToMe |> List.filter (fun ud -> ud <> aUserToRemoveDesc) 

            if newUserListCheck |> List.isEmpty then Error "Not found"
            else Ok {aGroupToRemoveFrom with UsersAddedToMe = newUserList }

        else

            let msg = sprintf "Wrong tenant consistency"

            Error msg

            
            
                             
                                                           























module Role = 



    open ServiceInterfaces
    open Group



    let toDescriptor (role:Role.Role) = 
        let roleDesc:RoleDescriptor = {
            Id = role.RoleId 
            TenantId = role.TenantId 
            Name = role.Name 
            ShortDesc = role.Description 
        }
        roleDesc 


    let create (roleId:string) (tenantId:string) (name:string) (description:string) : Result<Role,string> =
        
        let roleConstruct = result {

            let! ids = roleId |> RoleId.create' 
            let! tenantIds = tenantId |> TenantId.create' 
            let! names = name |> RoleName.create'
            let! descriptions = description |> RoleDescription.create' 
            
            let role:Role = {
                RoleId = ids
                TenantId = tenantIds
                Name = names
                Description = descriptions
                SupportNesting = SupportNestingStatus.Support
                GroupsThatPlayMe = []
                UsersThatPlayMe = []
                }
            return role
        }

        roleConstruct











    let assignGroup (aRole:Role.Role) (aGroup:Group.Group) =


       if(aRole.TenantId |> isSameTenancy aGroup.TenantId) then  

            let aGroupDesc = aGroup |> Group.toDescriptor

            let rsFind = aRole.GroupsThatPlayMe |> List.exists (fun gd -> aGroupDesc = gd)

            if rsFind then Error "Group already plays role"
            else 

                let newGroupsPlayingRoles = aRole.GroupsThatPlayMe @ [aGroupDesc]

                Ok ({ aRole with GroupsThatPlayMe = newGroupsPlayingRoles }, aGroupDesc)
       
       else Error "Wrong tenancy consistency"
        







    let assignUser (aRole:Role) (aUser:User) =



       if(aRole.TenantId |> isSameTenancy aUser.TenantId) then  

            let aUserDesc = aUser |> User.toDescriptor
            let rsFind = aRole.UsersThatPlayMe |> List.exists (fun ud -> aUserDesc = ud)

            if rsFind then Error "User already plays role"
            else 

                let newUsersPlayingRoles = aRole.UsersThatPlayMe @ [aUserDesc]

                Ok ({ aRole with UsersThatPlayMe = newUsersPlayingRoles }, aUserDesc)
       
       else Error "Wrong tenancy consistency"


 







    let isInRole (aRole:Role.Role) (aUser:User.User)  =

        let aUserDesc = 
            aUser 
            |> User.toDescriptor
            
        aRole.UsersThatPlayMe 
        |> List.exists (fun ud -> aUserDesc = ud)




        










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


    type RoleDescriptor = {
        Id: string
        TenantId: string
        Name: string
        ShortDesc : string
        }

    type GroupDescriptor = {
        Id : string
        TenantId : string
        Name : string
        ShortDesc : string 
        }
        
    type UserDescriptor = {
        UserDescriptorId: string
        TenantId: string
        Username: string
        Email: string
        FirstName: string
        LastName: string     
        }

    ////////////////
 
    


    type User = {
        UserId: string
        TenantId: string
        Username: string
        Password: string
        Enablement: Enablement
        Person: Person
        RolesIPlay : RoleDescriptor list
        GroupsIAmIn : GroupDescriptor list
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

    type Group = {
        GroupId: string
        TenantId: string
        Name: string
        Description: string
        UsersAddedToMe: UserDescriptor list
        GroupsAddedToMe : GroupDescriptor list
        GroupsIamAddedTo: GroupDescriptor list
        RolesIPlay : RoleDescriptor list
        }


   


    type GroupCreated = Group
       


    type UserAddedToGroupEvent = {
        GroupId : string
        TenantId : string
        UserId : string 
        AddedUser : UserDescriptor
    }



    type UserRemovedFromGroupEvent = {
        GroupId : string
        TenantId : string
        UserId : string 
        RemovedUser : UserDescriptor
    }


    type UserAddedToGroup = {

        UserAdded : User
    }
    
    type GroupAddedToGroup = {

        GroupAdded : GroupCreated
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


    

    type GroupAddedToGroupEvent = { 
        GroupId : string
        TenantId : string
        MemberAdded : GroupMember
        }

    type GroupInAddedToGroupEvent = { 
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
        GroupsThatPlayMe: GroupDescriptor list
        UsersThatPlayMe: UserDescriptor list
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
        AssignedUser : UserDescriptor
        }

    type UserUnAssignedFromRoleEvent = { 
        RoleId : string
        UserId : string
        AssignedUser : UserDescriptor
        }

    type GroupAssignedToRoleEvent = { 
        RoleId : string
        GroupId : string
        TenantId : string
        AssignedGroup : GroupDescriptor
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



    module RoleDescriptor = 

        let fromDomain (aRoleDescriptor:User.RoleDescriptor) :RoleDescriptor =

            let aRoleDescriptorDto:RoleDescriptor = {
                Id = aRoleDescriptor.Id |> RoleId.value
                TenantId = aRoleDescriptor.TenantId |> TenantId.value
                Name = aRoleDescriptor.Name |> RoleName.value
                ShortDesc = aRoleDescriptor.ShortDesc |> RoleDescription.value
            }
            aRoleDescriptorDto



        let toDomain (aRoleDescriptor:RoleDescriptor) : Result<User.RoleDescriptor, string> =

            result {

                let! roleId = aRoleDescriptor.Id |> RoleId.create'
                let! tenantId = aRoleDescriptor.TenantId |> TenantId.create'
                let! name = aRoleDescriptor.Name |> RoleName.create'
                let! desc = aRoleDescriptor.ShortDesc |> RoleDescription.create'

                let aRoleDescriptorDomain:User.RoleDescriptor = {
                    Id = roleId
                    TenantId = tenantId
                    Name = name
                    ShortDesc = desc
                }
                return aRoleDescriptorDomain
            }


    module GroupDescriptor = 

        let fromDomain (aGroupDescriptor:User.GroupDescriptor) :GroupDescriptor =

            let aGroupDescriptorDto:GroupDescriptor = {
                Id = aGroupDescriptor.Id |> GroupId.value
                TenantId = aGroupDescriptor.TenantId |> TenantId.value
                Name = aGroupDescriptor.Name |> GroupName.value
                ShortDesc = aGroupDescriptor.ShortDesc |> GroupDescription.value
            }
            aGroupDescriptorDto



        let toDomain (aGroupDescriptor:GroupDescriptor) : Result<User.GroupDescriptor, string> =

            result {

                let! groupId = aGroupDescriptor.Id |> GroupId.create'
                let! tenantId = aGroupDescriptor.TenantId |> TenantId.create'
                let! name = aGroupDescriptor.Name |> GroupName.create'
                let! desc = aGroupDescriptor.ShortDesc |> GroupDescription.create'

                let aGroupDescriptorDomain:User.GroupDescriptor = {
                    Id = groupId
                    TenantId = tenantId
                    Name = name
                    ShortDesc = desc
                }
                return aGroupDescriptorDomain
            }





    module UserDescriptor = 

        let fromDomain (aUserDescriptor:User.UserDescriptor) :UserDescriptor =

            let aUserDescriptor:UserDescriptor = {
                UserDescriptorId = aUserDescriptor.UserId |> UserId.value
                TenantId = aUserDescriptor.TenantId |> TenantId.value
                Username = aUserDescriptor.Username |> Username.value
                Email = aUserDescriptor.Email |> EmailAddress.value
                FirstName = aUserDescriptor.FirstName |> FirstName.value
                LastName = aUserDescriptor.LastName |> LastName.value
            }
            aUserDescriptor



        let toDomain (aUserDescriptor:UserDescriptor) : Result<User.UserDescriptor, string> =

            result {

                let! userId = aUserDescriptor.UserDescriptorId |> UserId.create'
                let! tenantId = aUserDescriptor.TenantId |> TenantId.create'
                let! username = aUserDescriptor.Username |> Username.create'
                let! email = aUserDescriptor.Email |> EmailAddress.create'
                let! firstName = aUserDescriptor.FirstName |> FirstName.create'
                let! lastName = aUserDescriptor.LastName |> LastName.create'

                let aGroupDescriptorDomain:User.UserDescriptor = {
                    UserId = userId
                    TenantId = tenantId
                    Username = username
                    Email = email
                    FirstName = firstName
                    LastName = lastName
                }
                return aGroupDescriptorDomain
            }




    module User =


        let fromDomain (aUser:User.User) : User =

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

            let dtoRoles = 
                aUser.RolesIPlay
                |> List.map RoleDescriptor.fromDomain

            let dtoGroupsIamIn = 
                aUser.GroupsIAmIn
                |> List.map GroupDescriptor.fromDomain

            let userDto:User = {
                UserId = aUser.UserId |> UserId.value
                TenantId = aUser.TenantId |> TenantId.value
                Username = aUser.Username  |> Username.value
                Password = aUser.Password  |> Password.value
                Enablement = enablement 
                Person = person
                RolesIPlay = dtoRoles
                GroupsIAmIn = dtoGroupsIamIn
                }

            userDto




        let toDomain (aUserDto:User) : Result<User.User, string> =

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

                let! domainRoles = 
                     aUserDto.RolesIPlay
                     |> List.map RoleDescriptor.toDomain
                     |> ResultOfSequenceTemp

                let! domainGroupsIamIn = 
                     aUserDto.GroupsIAmIn
                     |> List.map GroupDescriptor.toDomain
                     |> ResultOfSequenceTemp

                let user: IdentityAndAcccess.DomainTypes.User.User = {
                    UserId = userId
                    TenantId = tenantId
                    Username = username
                    Password = password
                    Enablement = enablement
                    Person = person
                    RolesIPlay = domainRoles
                    GroupsIAmIn = domainGroupsIamIn
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
           
            let usersAddedToMe = 
                aGroup.UsersAddedToMe 
                |> List.map UserDescriptor.fromDomain


            let groupsAddedTome = 
                aGroup.GroupsAddedToMe
                |> List.map GroupDescriptor.fromDomain


            let groupsIamAddedTo = 
                aGroup.GroupsIamAddedTo
                |> List.map GroupDescriptor.fromDomain
                
            let rolesIPlay = 
                aGroup.RolesIPlay
                |> List.map RoleDescriptor.fromDomain


            let group:Group = {
                GroupId = aGroup.GroupId |>  GroupId.value
                TenantId = aGroup.TenantId |>  TenantId.value
                Name = aGroup.Name |>  GroupName.value
                Description = aGroup.Description |>  GroupDescription.value

                UsersAddedToMe = usersAddedToMe
                GroupsAddedToMe = groupsAddedTome
                GroupsIamAddedTo = groupsIamAddedTo
                RolesIPlay = rolesIPlay

                }
            group



        let toDomain (aGrouDto:Group) : Result<IdentityAndAcccess.DomainTypes.Group.Group, string> =

            result {

                let! groupId = aGrouDto.GroupId |> GroupId.create'
                let! tenantId = aGrouDto.TenantId |> TenantId.create'
                let! name = aGrouDto.Name |> GroupName.create'
                let! description =  aGrouDto.Description |>  GroupDescription.create'

                let! usersAddedToMe = 
                    aGrouDto.UsersAddedToMe 
                    |> List.map UserDescriptor.toDomain
                    |> ResultOfSequenceTemp


                let! groupsAddedTome = 
                     aGrouDto.GroupsAddedToMe
                     |> List.map GroupDescriptor.toDomain
                     |> ResultOfSequenceTemp


                let! groupsIamAddedTo = 
                     aGrouDto.GroupsIamAddedTo
                     |> List.map GroupDescriptor.toDomain
                     |> ResultOfSequenceTemp
                    
                let! rolesIPlay = 
                     aGrouDto.RolesIPlay
                     |> List.map RoleDescriptor.toDomain
                     |> ResultOfSequenceTemp

                                          
                let group: Group.Group = {
                    GroupId = groupId 
                    TenantId = tenantId
                    Name = name 
                    Description = description
                    UsersAddedToMe = usersAddedToMe
                    GroupsAddedToMe = groupsAddedTome
                    GroupsIamAddedTo = groupsIamAddedTo
                    RolesIPlay = rolesIPlay
                    }

               
                return group
                }

            





    module Role =


 
        let fromDomain (aRole:IdentityAndAcccess.DomainTypes.Role.Role) : Role =

            let supportNest = 
                match aRole.SupportNesting with  
                | Role.SupportNestingStatus.Support -> SupportNestingStatus.Support 
                | Role.SupportNestingStatus.Oppose -> SupportNestingStatus.Oppose

            let groups =
                aRole.GroupsThatPlayMe
                |> List.map GroupDescriptor.fromDomain

                
                 
            let users = 
                aRole.UsersThatPlayMe
                |> List.map UserDescriptor.fromDomain


            let role:Role = {
                RoleId =  aRole.RoleId |> RoleId.value
                TenantId = aRole.TenantId |>  TenantId.value
                Name = aRole.Name |>  RoleName.value
                Description = aRole.Description |>  RoleDescription.value
                SupportNesting = supportNest
                GroupsThatPlayMe = groups
                UsersThatPlayMe = users           
                }

            role




        let toDomain (aRoleDto:Role) : Result<IdentityAndAcccess.DomainTypes.Role.Role, string> =

            result {


                let! roleId = aRoleDto.RoleId |> RoleId.create'
                let! roleTenantId = aRoleDto.TenantId |> TenantId.create'
                let! roleName = aRoleDto.Name |> RoleName.create'
                let! roleDescription = aRoleDto.Description |> RoleDescription.create'
                let! supportNesting  = 
                    match aRoleDto.SupportNesting  with  
                    | SupportNestingStatus.Support -> Ok Role.SupportNestingStatus.Support
                    | SupportNestingStatus.Oppose -> Ok Role.SupportNestingStatus.Oppose
                    | _ -> Error "Unknown group member type"            


                let! groups =
                     aRoleDto.GroupsThatPlayMe
                     |> List.map GroupDescriptor.toDomain
                     |> ResultOfSequenceTemp

                    
                     
                let! users = 
                     aRoleDto.UsersThatPlayMe
                     |> List.map UserDescriptor.toDomain
                     |> ResultOfSequenceTemp


                let role : Role.Role = {
                    RoleId =  roleId
                    TenantId = roleTenantId
                    Name =roleName
                    Description = roleDescription
                    SupportNesting = supportNesting
                    GroupsThatPlayMe = groups
                    UsersThatPlayMe = users
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


        let fromDomain (aTenant:Tenant.Tenant) : Tenant =


            let status = 
                match aTenant.ActivationStatus with 
                | IdentityAndAcccess.DomainTypes.Tenant.ActivationStatus.Activated ->  ActivationStatus.Activated 
                | IdentityAndAcccess.DomainTypes.Tenant.ActivationStatus.Deactivated  ->  ActivationStatus.Deactivated 

      
            let regins = aTenant.RegistrationInvitations |> List.map RegistrationInvitation.fromDomain

            let tenant:Tenant = {
                TenantId = aTenant.TenantId |> TenantId.value
                Name = aTenant.Name |> TenantName.value
                Description = aTenant.Description |> TenantDescription.value
                RegistrationInvitations = regins
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