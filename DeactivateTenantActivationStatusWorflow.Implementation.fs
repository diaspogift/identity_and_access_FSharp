module IdentityAndAcccess.DeactivateTenantActivationStatusApiTypes.DeactivateTenantActivationStatusWorflowImplementation

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.Workflow.DeactivateTenantActivationStatusApiTypes
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainServicesImplementations


open IdentityAndAcccess.Workflow.DeactivateTenantActivationStatusApiTypes
open IdentityAndAcccess.DomainTypes.Functions.Dto





///Dependencies 
/// 
/// 












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






///Step 1 Validation of TenantActivationStatus  impl
let validateTenantActivationStatus : ValidateTenantActivationStatus =

    fun aUnvalidatedTenantActivationStatus ->

        result {


            let! tenantId = aUnvalidatedTenantActivationStatus.TenantId 
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

        
        


     
        







 
