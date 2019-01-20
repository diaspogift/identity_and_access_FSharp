namespace IdentityAndAcccess.Workflow.DeactivateTenantActivationStatusApiTypes

open IdentityAndAcccess.DomainTypes.Tenant


open System
open IdentityAndAcccess.DomainTypes
open IdentityAndAccess.DatabaseTypes








///Deactivate tenant activation status worflow types
/// 
/// 


//Deactivate tenant activation status tenant workflow input types
type UnvalidatedTenantActivationStatus = {
    
    TenantId : string
    ActivationStatus : bool
    Reason : string
}






//Defining the comand types
type Command<'data> = {
    Data : 'data;
    TimeStamp : DateTime;
    UserId : string;
}




type DeactivateTenantActivationStatusCommand =
        Command<UnvalidatedTenantActivationStatus> 


///Ouputs of the provision tenant worflow 
//- Sucess types




type TenantActivationStatusDeactivated = TenantActivationStatusDeactivated  of  Tenant



type TenantActivationStatusDeactivatedEvent = {

    Tenant : TenantDto
    ActivationStatus : ActivationStatusDto
    Reason : string
}
   

type TenantActivationStatusReactivatedEvent = {

    Tenant : TenantDto
    ActivationStatus : ActivationStatusDto
    Reason : string
}
   




//- Failure types

type DeactivateTenantActivationStatusError = 
    | ValidationError of string
    | DeactivationError of string
    | DbError of string


//Worflow type 

type DeactivateTenantActivationStatusWorkflow = 
    Tenant -> UnvalidatedTenantActivationStatus -> Result<TenantActivationStatusDeactivatedEvent, DeactivateTenantActivationStatusError>
