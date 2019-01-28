namespace IdentityAndAcccess.Workflow.ProvisionTenantApiTypes



open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.CommonDomainTypes

open System
open IdentityAndAcccess.DomainTypes








///Provision tenant worflow types
/// 
/// 


//provision tenant workflow input types
type UnvalidatedTenantProvision = {

    TenantInfo : UnvalidatedTenant
    AdminUserInfo : TenantAdministrator
}

and  UnvalidatedTenant = {
    Name : string
    Description : string
}

and TenantAdministrator = {
    FirstName : string
    MiddleName : string
    LastName : string
    Email : string
    Address : string
    PrimPhone : string 
    SecondPhone : string
}


//Defining the comand types
type Command<'data> = {
    Data : 'data;
    TimeStamp : DateTime;
    UserId : string;
}




type ProvisionTenantCommand =
        Command<UnvalidatedTenantProvision> 


///Ouputs of the provision tenant worflow 
//- Sucess types




type TenantProvisionCreated = {
    TenantProvisioned : Tenant
    RoleProvisioned : Role
    UserRegistered : User
}

type InvitationWithdrawn = {
    TenantId : string
    Invitation : Dto.RegistrationInvitation
}

type InvitationOffered = {
    TenantId : string
    Invitation : Dto.RegistrationInvitation
}


type ProvisionAcknowledgementSent = {
        TenantId : TenantId
        Email : EmailAddress
}


type TenantProvisionedEvent = 
    | TenantProvisionCreated of TenantProvisionCreated
    | InvitationWithdrawn of InvitationWithdrawn
    | InvitationOffered of InvitationOffered
    | ProvisionAcknowledgementSent of ProvisionAcknowledgementSent



//- Failure types

type ProvisionTenantError = 
    | ValidationError of string
    | ProvisioningError of string
    | DbError of string


//Worflow type 

type ProvisionTenantWorkflow = 
    UnvalidatedTenantProvision -> Result<TenantProvisionedEvent list, ProvisionTenantError>
