namespace IdentityAndAcccess.DomainTypes


///Imported local libs
open IdentityAndAcccess.CommonDomainTypes
open System
open System.Text.RegularExpressions



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

type Person = {
    Contact: ContactInformation
    Name: FullName 
    Tenant: TenantId
    User: UserId
    }

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

type Group = {
    GroupId: GroupId
    TenantId: TenantId
    Name: GroupName
    Description: GroupDescription
    Members: GroupMember list
    }


type GroupIdOrGroupMemberId = 
    | GroupId of GroupId
    | GroupMemberId of GroupMemberId

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














///Should be a common domain type 
type MyTimeSpan = {
    Start: DateTime
    End: DateTime 
}


///Temporary types should there be here? let alone should they have been created?
type GroupMemberDtoTemp = {
    MemberId : string
    TenantId : string
    Name : string
    Type : string
    }

type RegistrationInvitationDtoTemp = {
    RegistrationInvitationId : string
    TenantId : string
    Description : string
    StartingOn: DateTime
    Until : DateTime
    }

            
            

            
        

        

   