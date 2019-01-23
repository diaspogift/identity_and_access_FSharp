module IdentityAndAcccess.WithdrawRegistrationInvitationApiTypes.WithdrawRegistrationInvitationWorflowImplementation

open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainServicesImplementations.Tenant
open IdentityAndAcccess.Workflow.WithdrawRegistrationInvitationApiTypes
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.DomainTypes.Functions.RegistrationInvitations
open IdentityAndAcccess.DomainTypes
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open IdentityAndAccess.DatabaseTypes





///Dependencies 
/// 
/// 

let loadOneTenantById : LoadTenantById = TenantDb.loadOneTenantById
let updateTenant : UpdateOneTenant = TenantDb.updateOneTenant














//Step 1 Validation of RegistrationInvitationDescription

type ValidatedRegistrationInvitationIdentifier = {

   RegistrationInvitationId : RegistrationInvitationId
   TenantId : TenantId
}



type ValidateRegistrationInvitationIdentifier =

    UnvalidatedRegistrationInvitationIdentifier -> Result<ValidatedRegistrationInvitationIdentifier, WithdrawRegistrationInvitationError>







//Step 2 Withdraw registration invitation

type WithdrawRegistrationInvitation = 

    Tenant -> ValidatedRegistrationInvitationIdentifier -> Result<(Tenant*RegistrationInvitation), WithdrawRegistrationInvitationError>






///Step 1 validate RegistrationInvitationIdentifier  impl
let validateRegistrationInvitationIdentifier : ValidateRegistrationInvitationIdentifier =

    fun anUnvalidatedRegistrationInvitationIdentifier ->

        result {


            let! registrationInvitationId = 
                anUnvalidatedRegistrationInvitationIdentifier.RegistrationInvitationId
                |> RegistrationInvitationId.create'
                |> Result.mapError WithdrawRegistrationInvitationError.ValidationError


            let! tenantId = 
                anUnvalidatedRegistrationInvitationIdentifier.TenantId
                |> TenantId.create'
                |> Result.mapError WithdrawRegistrationInvitationError.ValidationError

            let validatedRegInv : ValidatedRegistrationInvitationIdentifier = 
                {RegistrationInvitationId = registrationInvitationId; TenantId = tenantId}

            return validatedRegInv


        }





///Step2 withdraw registration invitation  impl
let withdrawRegistrationInvitation : WithdrawRegistrationInvitation = 

    fun  aTenant validatedRegInvId ->
        validatedRegInvId.RegistrationInvitationId
        |> Tenant.withdrawRegistrationInvitation aTenant
        |> Result.mapError WithdrawRegistrationInvitationError.WithdrawInvitationError 

         
        

        



type CreateEvents = (Tenant*RegistrationInvitation) -> RegistrationInvitationWithdrawnEvent 

        


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

        let registrationInvitationOfferredEvent : RegistrationInvitationWithdrawnEvent = {
            Tenant = (tenant |> DbHelpers.fromTenantDomainToDto)
            RegistrationInvitation = regInvDto
        }


        registrationInvitationOfferredEvent




///Wthdraw registration invitation workflow implementation
/// 
/// 
let withdrawRegistrationInvitationWorkflow: WithdrawRegistrationInvitationWorkflow = 

    fun aTenant anUnvalidatedRegistrationInvitationIdentifier ->

        let withdrawRegistrationInvitation = withdrawRegistrationInvitation aTenant
        let withdrawRegistrationInvitation = Result.bind withdrawRegistrationInvitation
        let createEvents = Result.map createEvents




        anUnvalidatedRegistrationInvitationIdentifier
        |> validateRegistrationInvitationIdentifier
        |> withdrawRegistrationInvitation
        |> createEvents
        


     
        







 
