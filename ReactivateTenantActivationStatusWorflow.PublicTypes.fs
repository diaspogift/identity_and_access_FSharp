namespace IdentityAndAcccess.Workflow.ReactivateTenantActivationStatusApiTypes

open IdentityAndAcccess.DomainTypes.Tenant


open System
open IdentityAndAccess.DatabaseTypes



open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.DomainTypes

open IdentityAndAcccess




///Reactivate tenant activation status worflow types
/// 
/// 


//Reactivate tenant activation status tenant workflow input types
type UnvalidatedTenantActivationStatusData = {
    
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




type ReactivateTenantActivationStatusCommand =
        Command<UnvalidatedTenantActivationStatusData> 


///Ouputs of the reactivate tenant worflow 
//- Sucess types







type TenantActivationStatusReactivatedEvent = {
    TenantId : Dto.TenantId
    Status : Dto.ActivationStatus
    Reason : Dto.Reason

}
   



//- Failure types

type ReactivateTenantActivationStatusError = 
    | ValidationError of string
    | ReactivationError of string
    | DbError of string


//Worflow type 

type ReactivateTenantActivationStatusWorkflow = 
    Tenant.Tenant -> CommonDomainTypes.Reason -> UnvalidatedTenantActivationStatusData -> Result<TenantActivationStatusReactivatedEvent, ReactivateTenantActivationStatusError>
