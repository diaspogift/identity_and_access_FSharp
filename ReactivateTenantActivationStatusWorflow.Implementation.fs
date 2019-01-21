module IdentityAndAcccess.ReactivateTenantActivationStatusApiTypes.ReactivateTenantActivationStatusWorflowImplementation

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.Workflow.ReactivateTenantActivationStatusApiTypes
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainServices


open IdentityAndAccess.DatabaseTypes





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

    Tenant -> ValidatedTenantActivationStatus -> Result<Tenant, ReactivateTenantActivationStatusError>






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

    fun  aTenant validatedTenantStatus ->
        
        aTenant
        |> Tenant.activateTenant 
        |> Result.mapError ReactivateTenantActivationStatusError.ReactivationError 
     





type CreateEvents = Tenant -> TenantActivationStatusReactivatedEvent 

        


///Step4 create events impl
let createEvents : CreateEvents = 

    fun tenant ->

        

        let tenantActivationStatusReactivatedEvent : TenantActivationStatusReactivatedEvent = {
            Tenant =  tenant |> DbHelpers.fromTenantDomainToDto
            ActivationStatus = ActivationStatusDto.Disactivated
            Reason = "FIXTURE FOR NOW"
        }


        tenantActivationStatusReactivatedEvent







///Reactivate tenant activation status workflow implementation
/// 
/// 
let reactivateTenantActivationStatusWorkflow: ReactivateTenantActivationStatusWorkflow = 

    fun aTenant anUnvalidatedTenantActivationStatus ->

        let  reactivateTenantActivationStatus =  reactivateTenantActivationStatus aTenant
        let  reactivateTenantActivationStatus =  Result.bind reactivateTenantActivationStatus
        let createEvents =   Result.map createEvents

        anUnvalidatedTenantActivationStatus
        |> validateTenantActivationStatus
        |> reactivateTenantActivationStatus 
        |> createEvents

        
        


     
        







 
