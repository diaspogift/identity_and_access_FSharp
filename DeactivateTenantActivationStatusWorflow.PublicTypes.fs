namespace IdentityAndAcccess.Workflow.DeactivateTenantActivationStatusApiTypes

open IdentityAndAcccess.DomainTypes.Tenant


open System
open IdentityAndAcccess.DomainTypes


open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.DomainTypes.Functions.Dto.Other
open IdentityAndAcccess.DomainTypes.Functions.Dto
open IdentityAndAcccess





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







type TenantActivationStatusDeactivatedEvent = {
    TenantId : Dto.TenantId
    Status : Dto.ActivationStatus
    Reason : Dto.Reason
}
   


   




//- Failure types

type DeactivateTenantActivationStatusError = 
    | ValidationError of string
    | DeactivationError of string
    | DbError of string


//Worflow type 

type DeactivateTenantActivationStatusWorkflow = 
    Tenant.Tenant -> CommonDomainTypes.Reason -> UnvalidatedTenantActivationStatus -> Result<TenantActivationStatusDeactivatedEvent, DeactivateTenantActivationStatusError>
