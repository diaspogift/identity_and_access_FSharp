namespace IdentityAndAcccess.DomainTypes


///Imported local libs
open IdentityAndAcccess.CommonDomainTypes
open System







[<AutoOpen>]
module Events =





    ///Useful types
    /// 
    /// 
    /// 
    /// 
    /// 
    type DomainEvent<'T> = {
        
        EventVersion : int
        OccurredOn : DateTime
        Data : 'T

    }



















module Tenant =

    open Events
    ///Tenant related types
    /// 
    /// 
    /// 
    /// 
    type ActivationStatus = 
        |Activated 
        |Disactivated


    type RegistrationInvitation = {
        RegistrationInvitationId: RegistrationInvitationId
        Description: RegistrationInvitationDescription
        TenantId: TenantId
        StartingOn: DateTime
        Until: DateTime
        }


    type Tenant = {
        TenantId: TenantId
        Name: TenantName
        Description: TenantDescription 
        RegistrationInvitations: RegistrationInvitation list
        ActivationStatus : ActivationStatus
        }


    type TenantActivationStatusActivatedData = {
        EventVesion : int
        OccurredOn : DateTime
        TenantId : TenantId
        UserId : UserId
        }



    type TenantActivationStatusActivated = 
        DomainEvent<TenantActivationStatusActivatedData>


    type TenantActivationStatusDiactivatedData = {
        EventVesion : int
        OccurredOn : DateTime
        TenantId : TenantId
        UserId : UserId 
        }



    type TenantActivationStatusDiactivated = 
        DomainEvent<TenantActivationStatusDiactivatedData>



    type TenantAdministratorRegisteredData = {
        EventVersion : int
        OccurredOn : DateTime
        TenantId : string
        FullName : string
        EmailAddress : string
        TenantName : string
        temporaryPassword : string
        Username : string
        }


    type TenantAdministratorRegistered = 
        DomainEvent<TenantAdministratorRegisteredData>


    type TenantProvisioned = {
        EventVersion : int
        OccurredOn : DateTime
        TenantId : string
        }
      



    type RegistrationInvitationDtoTemp = {
        RegistrationInvitationId : string
        TenantId : string
        Description : string
        StartingOn: DateTime
        Until : DateTime
        }




















module User =







    ///User related types
    /// 
    /// 
    /// 
    /// 
    /// 
    type FullName = {
        First: FirstName
        Middle: MiddleName
        Last: LastName
        }

    type EnablementStatus = 
        |Enabled 
        |Disabled

    type Enablement = {
        EnablementStatus: EnablementStatus
        StartDate: DateTime
        EndDate: DateTime
        }



    type ContactInformation = {
        Email : EmailAddress
        Address: PostalAddress
        PrimaryTel: Telephone
        SecondaryTel: Telephone
        }



    type ContactInformationChangedData = {
        EventVersion: int
        OccurredOn : DateTime
        TenantId: string
        Username: string
        FirstName: string
        LastName: string
        ContactInformation : string
        }

    type ContactInformationChanged = 
        DomainEvent<ContactInformationChangedData>



    type Person = {
        Contact: ContactInformation
        Name: FullName 
        Tenant: TenantId
        User: UserId
        }


    type PersonalNameChangedData = {
        EventVersion: int
        OccurredOn : DateTime
        TenantId: string
        Username: string
        FirstName: string
        LastName: string
        }



    type PersonalNameChanged = 
        DomainEvent<PersonalNameChangedData>


    type User = {
        UserId: UserId
        TenantId: TenantId
        Username: Username
        Password: Password
        Enablement: Enablement
        Person: Person
        }

    type UserDescriptor = {
        UserDescriptorId: UserDescriptorId
        TenantId: TenantId
        Username: Username
        Email: EmailAddress
        }


    type UserEnablementChangedData = {
        EventVersion: int
        OccurredOn : DateTime
        TenantId: string
        Username: string
        }

    type UserEnablementChanged = 
            DomainEvent<UserEnablementChangedData>




    type UserPasswordChangedData = {
        EventVersion: int
        OccurredOn : DateTime
        TenantId: string
        Username: string
        }



    type UserPasswordChanged = 
            DomainEvent<UserPasswordChangedData>


    type UserRegisteredChangedData = {
        EventVersion: int
        OccurredOn : DateTime
        TenantId: string
        Username: string
        FirstName: string
        LastName: string
        }
    
 


    type UserRegisteredChanged = 
        DomainEvent<UserRegisteredChangedData>












module Group =






    ///Group related types
    /// 
    /// 
    /// 
    /// 
    /// 
    type GroupMemberType = 
        | UserGroupMember
        | GroupGroupMember

    type GroupMember = {
        MemberId: GroupMemberId
        TenantId: TenantId
        Name: GroupMemberName
        Type: GroupMemberType
        }

    type StandardGroup = {
        GroupId: GroupId
        TenantId: TenantId
        Name: GroupName
        Description: GroupDescription
        Members: GroupMember list
        }



    type Group = 
        | Standard of StandardGroup
        | Internal of StandardGroup



    type GroupAddedToGroup = {
        GroupId: GroupId
        TenantId: TenantId
        Name: GroupName
        Description: GroupDescription
        Members: GroupMember list
        }


    type GroupIdOrGroupMemberId = 
        | GroupId of GroupId
        | GroupMemberId of GroupMemberId



    ///Temporary types should there be here? let alone should they have been created?
    type GroupMemberDtoTemp = {
        MemberId : string
        TenantId : string
        Name : string
        Type : string
        }


















module Role =


    open Group




    ///Role related types
    /// 
    /// 
    /// 
    /// 
    type SupportNestingStatus = 
        | Support
        | Oppose

    type Role = {
        RoleId: RoleId
        TenantId: TenantId
        Name: RoleName
        Description: RoleDescription
        SupportNesting: SupportNestingStatus
        InternalGroup: Group
        }










///Miscellaniuous types
/// 
/// 
/// 
/// 
/// 
/// 
/// 
/// 







            
            

            
        

        

   