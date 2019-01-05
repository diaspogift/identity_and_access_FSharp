namespace IdentityAndAccess.DatabaseTypes

open System
open MongoDB.Bson





///Tenant related dto types
/// 
/// 
type RegistrationInvitationDto = {
    RegistrationInvitationId: BsonObjectId
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
    TenantId : BsonObjectId
    Name : string
    Description : string
    RegistrationInvitations : RegistrationInvitationDto array
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
    UserId : BsonObjectId
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
    GroupId : BsonObjectId
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
    RoleId : BsonObjectId
    TenantId : string
    Name : string
    Description : string
    SupportNesting : SupportNestingStatusDto
    Group : GroupDto
}


//TO DO 
//1-) Remove the noisy  BsonObjectId in this layer
