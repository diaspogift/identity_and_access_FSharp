namespace IdentityAndAcccess.Workflow.DeactivateTenantActivationStatusApiTypes

open System
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.DomainTypes


open IdentityAndAcccess.DomainTypes.Functions.Dto





///Dependencies 
/// 
/// 








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




//Step 1 Validation of TenantActivationStatus

type ValidatedTenantActivationStatus = {

   TenantId : CommonDomainTypes.TenantId
   TenantActivationStatus : bool
}



type ValidateTenantActivationStatus =

    UnvalidatedTenantActivationStatus -> Result<ValidatedTenantActivationStatus, DeactivateTenantActivationStatusError>







//Step 2 Deactivate tenant activation status

type DeactivateTenantActivationStatus = 

    Tenant.Tenant -> CommonDomainTypes.Reason ->  ValidatedTenantActivationStatus -> Result<Tenant.Tenant*CommonDomainTypes.Reason, DeactivateTenantActivationStatusError>





module DeactivateTenantActivationStatusWorflowImplementation =



    ///Step 1 Validation of TenantActivationStatus  impl
    let validateTenantActivationStatus : ValidateTenantActivationStatus =

        fun aUnvalidatedTenantActivationStatus ->

            result {

                let! tenantId = 
                    aUnvalidatedTenantActivationStatus.TenantId 
                    |> TenantId.create'
                    |> Result.mapError DeactivateTenantActivationStatusError.ValidationError

                let validatedTenantActivationStatus:ValidatedTenantActivationStatus = {
                    TenantId = tenantId
                    TenantActivationStatus = aUnvalidatedTenantActivationStatus.ActivationStatus
                }

                return validatedTenantActivationStatus


            }





    ///Step2 deactivate tenant activation status impl
    let deactivateTenantActivationStatus : DeactivateTenantActivationStatus = 

        fun  aTenant aReason validatedTenantStatus -> 
            aReason 
            |> Tenant.deactivateTenant aTenant
            |> Result.mapError DeactivateTenantActivationStatusError.DeactivationError 
         



    type CreateEvents = Tenant.Tenant*CommonDomainTypes.Reason -> TenantActivationStatusDeactivatedEvent 

            


    ///Step4 create events impl
    let createEvents : CreateEvents = 

        fun (tenant, reason)  ->


            let dtoReason:Dto.Reason = {
                Description = reason |> Reason.value 
                }

            let tenantActivationStatusDeactivatedEvent : TenantActivationStatusDeactivatedEvent = {
                TenantId = tenant.TenantId |> TenantId.value
                Status = Dto.ActivationStatus.Deactivated
                Reason = dtoReason
                }     
            tenantActivationStatusDeactivatedEvent







    ///Deactivate tenant activation status workflow implementation
    /// 
    /// 
    let deactivateTenantActivationStatusWorkflow: DeactivateTenantActivationStatusWorkflow = 

        fun aTenant aReason anUnvalidatedTenantActivationStatus ->

            let  deactivateTenantActivationStatus =  deactivateTenantActivationStatus aTenant
            let  deactivateTenantActivationStatus =  deactivateTenantActivationStatus aReason
            let  deactivateTenantActivationStatus =  Result.bind deactivateTenantActivationStatus
            let createEvents =   Result.map createEvents

            anUnvalidatedTenantActivationStatus
            |> validateTenantActivationStatus
            |> deactivateTenantActivationStatus
            |> createEvents

            
            


         
            







     
