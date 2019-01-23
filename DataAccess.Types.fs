namespace IdentityAndAccess.DatabaseTypes

open System
open MongoDB.Bson
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Functions





///Tenant related dto types
/// 
/// 
type RegistrationInvitationDto = {
    RegistrationInvitationId: string
    Description: string
    TenantId: string
    StartingOn: DateTime
    Until: DateTime
}





type ActivationStatusDto = 
    |Activated  = 1
    |Disactivated = 0


type DeactivationStatusDto = Disactivated

type TenantDto = {
    _id : string
    TenantId : string
    Name : string
    Description : string
    RegistrationInvitations : RegistrationInvitationDtoTemp array
    ActivationStatus : ActivationStatusDto
}



type TenantActivationStatusDeactivatedDto = {
    Tenant : TenantDto
    ActivationStatus: ActivationStatusDto 
    Reason : string
}


type TenantActivationStatusReactivatedDto = {
    Tenant : TenantDto
    ActivationStatus: ActivationStatusDto 
    Reason : string
}




type RegistrationInvitationOfferredDto = {
    TenantId : string
    Invitation: RegistrationInvitationDto
}


type RegistrationInvitationWithdrawnDto = {
    TenantId : string
    Invitation: RegistrationInvitationDto
}



type TenantCreatedDto = {
    _id : string
    TenantId : string
    Name : string
    Description : string
    RegistrationInvitations : RegistrationInvitationDtoTemp array
    ActivationStatus : ActivationStatusDto
}

///User related dto types
/// 
/// 
type EnablementStatusDto = 
    | Enabled = 1
    | Disabled = 2


type UserDto = {
    _id : string
    UserId : string
    TenantId: string
    Username: string
    Password: string
    EnablementStatus: EnablementStatusDto
    EnablementStartDate: DateTime
    EnablementEndDate: DateTime
    EmailAddress: string
    PostalAddress: string
    PrimaryTel: string
    SecondaryTel: string
    FirstName: string
    LastName: string
    MiddleName: string
}

type UserRegisteredEventDto = {
    _id : string
    UserId : string
    TenantId: string
    Username: string
    Password: string
    EnablementStatus: EnablementStatusDto
    EnablementStartDate: DateTime
    EnablementEndDate: DateTime
    EmailAddress: string
    PostalAddress: string
    PrimaryTel: string
    SecondaryTel: string
    FirstName: string
    LastName: string
    MiddleName: string
}





///Group related dto types
/// 
/// 
type GroupMemberTypeDto = 
    | Group = 1
    | User = 2


type GroupMemberDto = {
    MemberId: string
    TenantId: string
    Name: string
    Type: GroupMemberTypeDto
}

type GroupDto = {
    _id : string
    GroupId : string
    TenantId :String
    Name : string
    Description : string
    Members : GroupMemberDto array
}

type GroupEventDto = {
    _id : string
    GroupId : string
    TenantId :String
    Name : string
    Description : string
    Members : GroupMemberDto array
}


type UserAddedToGroupDto = {
    Group : GroupDto
    User : UserDto
}



///Role related dto types
/// 
/// 
type SupportNestingStatusDto = 
    | Support = 1
    | Oppose = 0


type RoleDto = {
    _id : string
    RoleId : string
    TenantId : string
    Name : string
    Description : string
    SupportNesting : SupportNestingStatusDto
    Group : GroupDto
}

type RoleProvisionedEventDto = {
    _id : string
    RoleId : string
    TenantId : string
    Name : string
    Description : string
    SupportNesting : SupportNestingStatusDto
    Group : GroupDto
}



//TO DO 
//1-) Remove the noisy  BsonObjectId in this layer

