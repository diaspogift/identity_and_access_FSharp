namespace IdentityAndAcccess.Aggregates.Implementation
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainServices
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAccess.DatabaseTypes
open FSharp.Data.Sql
open System
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes





 
type Aggregate<'TState, 'TCommand, 'TEvent> = {
    
    zero : 'TState
    apply : 'TState option -> Result<'TEvent, string> -> 'TState
    exec : 'TState -> 'TCommand -> 'TEvent
}










/// Tenant aggregate related commands
type TenantCommand = 
    | CreateTenant of TenantDto
    | DeactivateTenant of TenantDto
    | ReactivateTenant of TenantDto
    | OffertRegistrationInvitation of RegistrationInvitation


type TenantEvents = 
    | TenantCreated of TenantDto
    | TenantDeactivated of TenantDto
    | TenantReactivated of TenantDto
    | RegistrationInvitationOfferred of RegistrationInvitationOfferredDto
    | RegistrationInvitationWithdrawn of RegistrationInvitationDto











module Tenant = 


    open IdentityAndAcccess.DomainTypes.Tenant








    let apply (aTenantCurrentState:TenantDto option) (aTenantEvent:TenantEvents) : Result<TenantDto, string> =

        match aTenantEvent with  
        | TenantEvents.TenantCreated createdTenant ->
             Ok createdTenant

        | TenantEvents.TenantDeactivated tenant ->

            match aTenantCurrentState with  
            | Some tenant ->

                if tenant.ActivationStatus = ActivationStatusDto.Disactivated then 
                    Error "Tenant already deactivated"
                else
                    Ok {tenant with ActivationStatus = ActivationStatusDto.Disactivated}

            | None ->

                Error "No tenant given"

        | TenantEvents.TenantReactivated  tenant ->

            match aTenantCurrentState with  
            | Some tenant ->
                if tenant.ActivationStatus = ActivationStatusDto.Activated then 
                    Error "Tenant already activated"
                else
                    Ok {tenant with ActivationStatus = ActivationStatusDto.Activated}
            | None ->

                Error "No tenant given"

        | TenantEvents.RegistrationInvitationOfferred aRegistrationInvitation  ->
           
            match aTenantCurrentState with  
            | Some tenant ->


                let aRegistrationInvitationList = 
                               [aRegistrationInvitation] 
                               |> List.toArray
                               |> Array.map (
                                   fun regIn -> 
                                        let  registrationInvitationDtoTemp : RegistrationInvitationDtoTemp = {
                                          RegistrationInvitationId = regIn.Invitation.RegistrationInvitationId  
                                          TenantId = regIn.TenantId 
                                          Description = regIn.Invitation.Description 
                                          StartingOn = regIn.Invitation.StartingOn
                                          Until = regIn.Invitation.Until
                                        }

                                        registrationInvitationDtoTemp
                               )



                let invitationsDtoTempList =  Array.append aRegistrationInvitationList  tenant.RegistrationInvitations


                Ok {tenant with RegistrationInvitations = invitationsDtoTempList }

            | None ->

                Error "No tenant given"


        | TenantEvents.RegistrationInvitationWithdrawn aRegistrationInvitation  ->
       
            match aTenantCurrentState with  
            | Some tenant ->


                let aRegistrationInvitationList = 
                               [aRegistrationInvitation] 
                               |> List.toArray
                               |> Array.map (
                                   fun regIn -> 
                                        let  registrationInvitationDtoTemp : RegistrationInvitationDtoTemp = {
                                          RegistrationInvitationId = regIn.RegistrationInvitationId  
                                          TenantId = regIn.TenantId 
                                          Description = regIn.Description 
                                          StartingOn = regIn.StartingOn
                                          Until = regIn.Until
                                        }

                                        registrationInvitationDtoTemp
                               )



                let invitationsDtoTempList =  Array.append aRegistrationInvitationList  tenant.RegistrationInvitations


                Ok {tenant with RegistrationInvitations = invitationsDtoTempList }

            | None ->

                Error "No tenant given"

            


    (* let exec (aTenant:TenantDto) (aCommand:TenantCommand) =

        match aCommand with  
        | CreateTenant tenant ->
             tenant
        | DeactivateTenant tenant ->

            let deactivateTenant = 
                tenant 
                |> DbHelpers.fromDbDtoToTenant 
                |> Result.bind Tenant.deactivateTenant 

            deactivateTenant

            match tenant with 
            | Ok t -> t
            | Error error -> aTenant
            



            tenant
            
            
        | ReactivateTenant tenant ->
            tenant
            |> Tenant.deactivateTenant 

            tenant
    
        | OffertRegistrationInvitation regInv ->

             {aTenant with RegistrationInvitations = [regInv] @ aTenant.RegistrationInvitations} 



    type Id = System.Guid

    /// Creates a persistent, async command handler for an aggregate given load and commit functions.
    let makeHandler (aggregate:Aggregate<'TState, 'TCommand, 'TEvent>) (load:System.Type * Id -> Async<obj seq>, commit:Id * int -> obj -> Async<unit>) =
        fun (id,version) command -> async {
            let! events = load (typeof<'TEvent>,id)
            let events = events |> Seq.cast :> 'TEvent seq
            let state = Seq.fold aggregate.apply aggregate.zero events
            let event = aggregate.exec state command
            match event with
            | Choice1Of2 event ->
                let! _ = event |> commit (id,version)
                return Choice1Of2 ()
            | Choice2Of2 errors -> 
                return errors |> Choice2Of2
        }

    /// Creates a persistent command handler for an aggregate given load and commit functions.
    let makeHandlerSync (aggregate:Aggregate<'TState, 'TCommand, 'TEvent>) (load:System.Type * Id -> obj seq, commit:Id * int -> obj -> unit) =
        fun (id,version) command ->
            let events = load (typeof<'TEvent>,id) |> Seq.cast :> 'TEvent seq
            let state = Seq.fold aggregate.apply aggregate.zero events
            let result = aggregate.exec state command
            match result with
            | Choice1Of2 event  -> event |> commit (id,version)   |> Choice1Of2 
            | Choice2Of2 errors -> errors |> Choice2Of2 *)