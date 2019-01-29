module IdentityAndAcccess.ReactivateTenantActivationStatusApiTypes.ReactivateTenantActivationStatusWorflowImplementation

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.Workflow.ReactivateTenantActivationStatusApiTypes
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.DomainTypes







///Dependencies 
/// 
/// 

//let loadOneTenantById : LoadTenantById = TenantDb.loadOneTenantById
//let updateTenant : UpdateOneTenant = TenantDb.updateOneTenant














//Step 1 Validation of TenantActivationStatus

type ValidatedTenantActivationStatus = {

   TenantId : TenantId
   TenantActivationStatus : bool
}



type ValidateTenantActivationStatus =

    UnvalidatedTenantActivationStatusData -> Result<ValidatedTenantActivationStatus, ReactivateTenantActivationStatusError>







//Step 2 Reactivate tenant activation status

type ReactivateTenantActivationStatus = 

    Tenant.Tenant -> CommonDomainTypes.Reason  -> ValidatedTenantActivationStatus -> Result<Tenant.Tenant*CommonDomainTypes.Reason, ReactivateTenantActivationStatusError>






///Step 1 Validation of TenantActivationStatus  impl
let validateTenantActivationStatus : ValidateTenantActivationStatus =

    fun aUnvalidatedTenantActivationStatus ->

        result {


            let! tenantId = aUnvalidatedTenantActivationStatus.TenantId 
                            |> TenantId.create'
                            |> Result.mapError ReactivateTenantActivationStatusError.ValidationError

            let validatedTenantActivationStatus:ValidatedTenantActivationStatus = {

                TenantId = tenantId
                TenantActivationStatus = aUnvalidatedTenantActivationStatus.ActivationStatus

            }

            return validatedTenantActivationStatus


        }





///Step2 reactivate tenant activation status impl
let reactivateTenantActivationStatus : ReactivateTenantActivationStatus = 

    fun  aTenant aReason validatedTenantStatus ->    
        aReason 
        |> Tenant.activateTenant aTenant
        |> Result.mapError ReactivateTenantActivationStatusError.ReactivationError 
     





type CreateEvents = Tenant.Tenant*Reason -> TenantActivationStatusReactivatedEvent 

        


///Step4 create events impl
let createEvents : CreateEvents = 

    fun (tenant, reason) ->


        let dtoReason:Dto.Reason = {
            Description = reason |> Reason.value
            }

        let tenantActivationStatusReactivatedEvent : TenantActivationStatusReactivatedEvent = {
            TenantId = tenant.TenantId |> TenantId.value
            Status = Dto.ActivationStatus.Activated
            Reason = dtoReason
            }


        tenantActivationStatusReactivatedEvent










///Reactivate tenant activation status workflow implementation
/// 
/// 
let reactivateTenantActivationStatusWorkflow: ReactivateTenantActivationStatusWorkflow = 

    fun aTenant aReason anUnvalidatedTenantActivationStatus ->

        let  reactivateTenantActivationStatus =  reactivateTenantActivationStatus aTenant
        let  reactivateTenantActivationStatus =  reactivateTenantActivationStatus aReason
        let  reactivateTenantActivationStatus =  Result.bind reactivateTenantActivationStatus
        let createEvents =   Result.map createEvents

        anUnvalidatedTenantActivationStatus
        |> validateTenantActivationStatus
        |> reactivateTenantActivationStatus 
        |> createEvents

        
        


     
        







 
