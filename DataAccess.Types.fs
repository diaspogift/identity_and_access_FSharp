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


type TenantDto = {
    _id : BsonObjectId
    TenantId : string
    Name : string
    Description : string
    RegistrationInvitations : RegistrationInvitationDtoTemp array
    ActivationStatus : ActivationStatusDto
}


type RegistrationInvitationOfferredEventDto = {
    Tenant : TenantDto
    Invitation: RegistrationInvitationDto
}

type TenantProvisionedEventDto = {
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
    _id : BsonObjectId
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
    MemberId: BsonObjectId
    TenantId: string
    Name: string
    Type: GroupMemberTypeDto
}

type GroupDto = {
    _id : BsonObjectId
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






///Role related dto types
/// 
/// 
type SupportNestingStatusDto = 
    | Support = 1
    | Oppose = 0


type RoleDto = {
    _id : BsonObjectId
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

