namespace IdentityAndAcccess.DomainApiTypes

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.CommonDomainTypes

open System








///Provision tenant worflow types
/// 
/// 


//provision tenant workflow input types
type UnvalidatedTenantProvision = {

    TenantInfo : UnvalidatedTenant
    AdiminUserInfo : TenantAdministrator
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




type ProvisionTenant =
        Command<UnvalidatedTenantProvision> 


///Ouputs of the provision tenant worflow 
//- Sucess types




type TenantProvisionCreated = {
    TenantProvisioned : Tenant
    RoleProvisioned : Role
    UserCreated : User
}

type ProvisionAcknowledgementSent = {
        TenantId : TenantId
        Email : EmailAddress
}


type TenantProvisionedEvent = 
    | TenantProvisionCreated of TenantProvisionCreated
    | ProvisionAcknowledgementSent of ProvisionAcknowledgementSent



//- Failure types

type ProvisionTenantError = 
    | ValidationError of string
    | ProvisioningError of string
    | DbError of string


//Worflow type 

type ProvisionTenantWorkflow = 
    UnvalidatedTenantProvision -> Result<TenantProvisionedEvent list, ProvisionTenantError>
