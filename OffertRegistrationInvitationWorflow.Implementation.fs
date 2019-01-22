module IdentityAndAcccess.OffertRegistrationInvitationApiTypes.OffertRegistrationInvitationWorflowImplementation

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainServices.Tenant
open IdentityAndAcccess.Workflow.OffertRegistrationInvitationApiTypes
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.DomainTypes

open IdentityAndAccess.DatabaseTypes
open IdentityAndAcccess.DomainServices




///Dependencies 
/// 
/// 

let loadOneTenantById : LoadTenantById = TenantDb.loadOneTenantById
let updateTenant : UpdateOneTenant = TenantDb.updateOneTenant














//Step 1 Validation of RegistrationInvitationDescription

type ValidatedRegistrationInvitationDescription = {

   TenantId : TenantId
   RegistrationInvitationDescription : RegistrationInvitationDescription
}



type ValidateRegistrationInvitationDescription =

    UnvalidatedRegistrationInvitationDescription -> Result<ValidatedRegistrationInvitationDescription, OfferRegistrationInvitationError>







//Step 2 Offer registration invitation

type OfferRegistrationInvitation = 

    Tenant -> ValidatedRegistrationInvitationDescription -> Result<(Tenant*RegistrationInvitation), OfferRegistrationInvitationError>






///Step 1 validate RegistrationInvitationDescription  impl
let validateRegistrationInvitationDescription : ValidateRegistrationInvitationDescription =

    fun aUnvalidatedRegistrationInvitationDescription ->

        result {


            let! description = 
                aUnvalidatedRegistrationInvitationDescription.Description
                |> RegistrationInvitationDescription.create'
                |> Result.mapError OfferRegistrationInvitationError.ValidationError


            let! tenantId = 
                aUnvalidatedRegistrationInvitationDescription.TenantId
                |> TenantId.create'
                |> Result.mapError OfferRegistrationInvitationError.ValidationError

            let validatedRegInv : ValidatedRegistrationInvitationDescription = {TenantId = tenantId; RegistrationInvitationDescription = description}

            return validatedRegInv


        }





///Step2 offer registration invitation  impl
let offerRegistrationInvitation : OfferRegistrationInvitation = 

    fun  aTenant validatedRegInv ->

         
        
        validatedRegInv.RegistrationInvitationDescription
        |> Tenant.offerRegistrationInvitation aTenant
        |> Result.mapError OfferRegistrationInvitationError.OfferInvitationError 





type CreateEvents = (Tenant*RegistrationInvitation) -> RegistrationInvitationOfferredEvent 

        


///Step4 create events impl
let createEvents : CreateEvents = 

    fun tenantAndInvitation ->

        let tenant, registrationInvitation =  tenantAndInvitation


        let regInvDto:RegistrationInvitationDto = {

            RegistrationInvitationId = registrationInvitation.RegistrationInvitationId |> RegistrationInvitationId.value
            Description = registrationInvitation.Description |> RegistrationInvitationDescription.value
            TenantId = registrationInvitation.TenantId |> TenantId.value
            StartingOn = registrationInvitation.StartingOn
            Until = registrationInvitation.Until
        }

        let registrationInvitationOfferredEvent : RegistrationInvitationOfferredEvent = {
            Tenant = (tenant |> DbHelpers.fromTenantDomainToDto)
            RegistrationInvitation = regInvDto
        }


        registrationInvitationOfferredEvent




///Offer registration invitation workflow implementation
/// 
/// 
let offerRegistrationInvitationWorkflow: OfferRegistrationInvitationWorkflow = 

    fun aTenant anUnvalidatedRegistrationInvitationDescription ->

        let offerRegistrationInvitation = offerRegistrationInvitation aTenant
        let offerRegistrationInvitation = Result.bind offerRegistrationInvitation
        let createEvents = Result.map createEvents




        anUnvalidatedRegistrationInvitationDescription
        |> validateRegistrationInvitationDescription
        |> offerRegistrationInvitation
        |> createEvents
        


     
        







 
