namespace IdentityAndAcccess.Workflow.WithdrawRegistrationInvitationApiTypes

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.CommonDomainTypes

open System
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainServicesImplementations
open IdentityAndAccess.DatabaseTypes


open IdentityAndAcccess.DomainTypes.Functions





///Withdraw registration invitation worflow types
/// 
/// 


//Withdraw registration invitation workflow input types
type UnvalidatedRegistrationInvitationIdentifier = {
   RegistrationInvitationId : string
   TenantId : string
}




//Defining the comand types
type Command<'data> = {
    Data : 'data;
    TimeStamp : DateTime;
    UserId : string;
}




type WithdrawRegistrationInvitationCommand =
        Command<UnvalidatedRegistrationInvitationIdentifier> 


///Ouputs of the withdraw registration invitation worflow 
//- Sucess types




type RegistrationInvitationWithdrawnEvent = {
    TenantId : Dto.TenantId
    WithdrawnInvitation : Dto.RegistrationInvitation
    }




//- Failure types

type WithdrawRegistrationInvitationError = 
    | ValidationError of string
    | WithdrawInvitationError of string
    | DbError of string


//Worflow type 

type WithdrawRegistrationInvitationWorkflow = 
    Tenant -> UnvalidatedRegistrationInvitationIdentifier -> Result<RegistrationInvitationWithdrawnEvent, WithdrawRegistrationInvitationError>
