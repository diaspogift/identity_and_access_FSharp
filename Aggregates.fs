namespace IdentityAndAcccess.Aggregates.Implementation
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.DomainTypes.Functions.Tenant
open IdentityAndAcccess.DomainServices
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainTypes.Functions






type Aggregate<'TState, 'TCommand, 'TEvent> = {
    zero : 'TState
    apply : 'TState -> 'TEvent -> 'TState
    exec : 'TState -> 'TCommand -> 'TEvent
}






/// Tenant aggregate related commands
type TenantCommand = 
    | CreateTenant of Tenant
    | DeactivateTenant
    | ReactivateTenant 
    | OffertRegistrationInvitation of RegistrationInvitation


type TenantEvents = 
    | TenantCreated of Tenant
    | TenantDeactivated 
    | TenantReactivated 
    | RegistrationInvitationOfferred of RegistrationInvitation








module Tenant = 


    let apply (aTenant:Tenant) (anEvent:TenantEvents) =

        match anEvent with  
        | TenantEvents.TenantCreated createdTenant ->
            Ok createdTenant

        | TenantEvents.TenantDeactivated  ->
            if aTenant.ActivationStatus = ActivationStatus.Disactivated then 
                Error "Tenant already deactivated"
            else
                Ok {aTenant with ActivationStatus = ActivationStatus.Disactivated}

        | TenantEvents.TenantReactivated  ->
            if aTenant.ActivationStatus = ActivationStatus.Activated then 
                Error "Tenant already activated"
            else
                Ok {aTenant with ActivationStatus = ActivationStatus.Activated}

        | TenantEvents.RegistrationInvitationOfferred aRegistrationInvitation  ->
            Ok {aTenant with RegistrationInvitations = [aRegistrationInvitation] @ aTenant.RegistrationInvitations}


    let exec (aTenant:Tenant) (aCommand:TenantCommand) =

        match aCommand with  
        | CreateTenant tenant ->
            Ok tenant
        | DeactivateTenant ->
            aTenant
            |> Tenant.deactivateTenant 
            
        | ReactivateTenant ->
            aTenant
            |> Tenant.deactivateTenant 
    
        | OffertRegistrationInvitation regInv ->
            Ok {aTenant with RegistrationInvitations = [regInv] @ aTenant.RegistrationInvitations}