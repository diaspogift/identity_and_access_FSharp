module IdentityAndAcccess.DeactivateTenantActivationStatusApiTypes.DeactivateTenantActivationStatusWorflowImplementation

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.Workflow.DeactivateTenantActivationStatusApiTypes
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainServices


open IdentityAndAcccess.Workflow.DeactivateTenantActivationStatusApiTypes
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open IdentityAndAccess.DatabaseTypes





///Dependencies 
/// 
/// 

let loadOneTenantById : LoadTenantById = TenantDb.loadOneTenantById
let updateTenant : UpdateOneTenant = TenantDb.updateOneTenant














//Step 1 Validation of TenantActivationStatus

type ValidatedTenantActivationStatus = {

   TenantId : TenantId
   TenantActivationStatus : bool
}



type ValidateTenantActivationStatus =

    UnvalidatedTenantActivationStatus -> Result<ValidatedTenantActivationStatus, DeactivateTenantActivationStatusError>







//Step 2 Deactivate tenant activation status

type DeactivateTenantActivationStatus = 

    Tenant -> ValidatedTenantActivationStatus -> Result<Tenant, DeactivateTenantActivationStatusError>






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

    fun  aTenant validatedTenantStatus ->
        
        aTenant
        |> Tenant.deactivateTenant 
        |> Result.mapError DeactivateTenantActivationStatusError.DeactivationError 
     





type CreateEvents = Tenant -> TenantActivationStatusDeactivatedEvent 

        


///Step4 create events impl
let createEvents : CreateEvents = 

    fun tenant ->

        

        let tenantActivationStatusDeactivatedEvent : TenantActivationStatusDeactivatedEvent = {
            Tenant =  tenant |> DbHelpers.fromTenantDomainToDto
            ActivationStatus = ActivationStatusDto.Disactivated
            Reason = "FIXTURE FOR NOW"
        }


        tenantActivationStatusDeactivatedEvent







///Offer registration invitation workflow implementation
/// 
/// 
let deactivateTenantActivationStatusWorkflow: DeactivateTenantActivationStatusWorkflow = 

    fun aTenant anUnvalidatedRegistrationInvitationDescription ->

        let  deactivateTenantActivationStatus =  deactivateTenantActivationStatus aTenant
        let  deactivateTenantActivationStatus =  Result.bind deactivateTenantActivationStatus
        let createEvents =   Result.map createEvents

        anUnvalidatedRegistrationInvitationDescription
        |> validateTenantActivationStatus
        |> deactivateTenantActivationStatus
        |> createEvents

        
        


     
        







 
