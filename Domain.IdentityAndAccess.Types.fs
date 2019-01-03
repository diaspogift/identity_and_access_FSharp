namespace IdentityAndAcccess.DomainTypes

open IdentityAndAcccess.CommonDomainTypes
open System



type SupportNestingStatus = 
    | Support
    | Oppose

type GroupMemberType = 
    | User
    | Group

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

type Role = {
    RoleId: RoleId
    TenantId: TenantId
    Name: RoleName
    Description: RoleDescription
    SupportNesting: SupportNestingStatus
    Group: Group
}


type FullName = {
    First: FirstName
    Middle: MiddleName
    Last: LastName
    }

type EnablementStatus = 
    |Enabled 
    |Disabled

type ActivationStatus = 
    |Activated 
    |Disactivated

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

type MyTimeSpan = {
    Start: DateTime
    End: DateTime 
}


type GroupMemberDtoTemp = {
    MemberId : string
    TenantId : string
    Name : string
    Type : string
    }
///Temp
/// 
/// 
/// 
/// 
/// 

type RegistrationInvitationDtoTemp = {
    RegistrationInvitationId: string
    TenantId: string
    Description: string
    StartingOn: DateTime
    Until: DateTime
}

            
            

            
        

        

   