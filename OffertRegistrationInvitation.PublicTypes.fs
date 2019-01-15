namespace IdentityAndAcccess.Workflow.OffertRegistrationInvitationApiTypes

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.CommonDomainTypes

open System
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainServices








///Offer registration invitation worflow types
/// 
/// 


//Offer registration invitation workflow input types
type UnvalidatedRegistrationInvitationDescription = {
   TenantId : string
   Description : string
}




//Defining the comand types
type Command<'data> = {
    Data : 'data;
    TimeStamp : DateTime;
    UserId : string;
}




type OfferRegistrationInvitationCommand =
        Command<UnvalidatedRegistrationInvitationDescription> 


///Ouputs of the offer registration invitation worflow 
//- Sucess types




type RegistrationInvitationOfferredEvent = {
    Tenant : Tenant
    RegistrationInvitation : RegistrationInvitation
    
}




//- Failure types

type OfferRegistrationInvitationError = 
    | ValidationError of string
    | OfferInvitationError of string
    | DbError of string


//Worflow type 

type OfferRegistrationInvitationWorkflow = 
    Tenant -> UnvalidatedRegistrationInvitationDescription -> Result<RegistrationInvitationOfferredEvent, OfferRegistrationInvitationError>
