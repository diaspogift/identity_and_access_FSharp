module IdentityAndAcccess.DomainApiTypes.Handlers


open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes.ProvisionTenantWorflowImplementation
open IdentityAndAcccess.Workflow.ProvisionGroupApiTypes
open IdentityAndAcccess.Workflow.ProvisionGroupApiTypes.ProvisionGroupWorflowImplementation
open IdentityAndAcccess.OffertRegistrationInvitationApiTypes.OffertRegistrationInvitationWorflowImplementation
open IdentityAndAcccess.Workflow.WithdrawRegistrationInvitationApiTypes
open IdentityAndAcccess.WithdrawRegistrationInvitationApiTypes.WithdrawRegistrationInvitationWorflowImplementation
open IdentityAndAcccess.DeactivateTenantActivationStatusApiTypes.DeactivateTenantActivationStatusWorflowImplementation
open IdentityAndAcccess.Workflow.ReactivateTenantActivationStatusApiTypes
open IdentityAndAcccess.ReactivateTenantActivationStatusApiTypes.ReactivateTenantActivationStatusWorflowImplementation

open IdentityAndAcccess.Workflow.DeactivateTenantActivationStatusApiTypes

open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainTypes
open FSharp.Data.Sql

open IdentityAndAcccess.DomainTypes.Tenant

open IdentityAndAcccess.Workflow.OffertRegistrationInvitationApiTypes
open IdentityAndAcccess.EventStorePlayGround.Implementation
open IdentityAndAcccess.EventStorePlayGround.Implementation.EventStorePlayGround


open IdentityAndAccess.DatabaseTypes


open System
open FSharp.Data.Sql
open System.Collections.Generic
open IdentityAndAcccess.Aggregates.Implementation
open IdentityAndAcccess.DomainTypes.Functions
open System.Collections.Generic
open FSharp.Data.Sql
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.Aggregates.Implementation

























module ProvisionTenant = 




    let allTenantAggregateEvents 
        (tenantProvisionedEventLis) 
        : (string * TenantStreamEvent array * string * RoleStreamEvent array * string * UserStreamEvent array) = 

        let mutable tenantEventList = Array.Empty()
        let mutable userEventList = Array.Empty()
        let mutable roleEventList = Array.Empty()
        let mutable tenantId = ""
        let mutable roleId = ""
        let mutable userId = ""
       

        tenantProvisionedEventLis
        |> List.map (
            
            fun event -> 


                match event with
                | TenantProvisionedEvent.TenantProvisionCreated aTenantProvisionCreated ->  


                    let tenantProvisioned = aTenantProvisionCreated.TenantProvisioned |> DbHelpers.fromTenantDomainToDto
                    let roleProvisioned = aTenantProvisionCreated.RoleProvisioned |> DbHelpers.fromRoleDomainToDto
                    let userRegistered = aTenantProvisionCreated.UserRegistered |> DbHelpers.fromUserDomainToDto

                    tenantId <-  concatTenantStreamId tenantProvisioned.TenantId
                    roleId <- concatRoleStreamId roleProvisioned.RoleId
                    userId <- concatUserStreamId userRegistered.UserId

                    let tenantCreatedEvent = tenantProvisioned |> TenantStreamEvent.TenantCreated 
                    let tenantCreatedEventL = tenantCreatedEvent |> List.singleton |> List.toArray


                    tenantEventList <- Array.append tenantEventList tenantCreatedEventL
           

                    let roleCreatedEvent = roleProvisioned |> RoleCreated 
                    let roleCreatedEventL = [roleCreatedEvent] |> List.toArray


                    roleEventList <- Array.append roleEventList roleCreatedEventL


                    let userRegisteredEvent = userRegistered |> UserRegistered
                    let userRegisteredEventL = [userRegisteredEvent] |> List.toArray

      
                    userEventList <- Array.append userEventList userRegisteredEventL


                | TenantProvisionedEvent.InvitationOffered invitaton ->


                        let ridto : RegistrationInvitationDto = {
                            RegistrationInvitationId = invitaton.Invitation.RegistrationInvitationId
                            Description = invitaton.Invitation.Description
                            TenantId = invitaton.TenantId
                            StartingOn = invitaton.Invitation.StartingOn
                            Until = invitaton.Invitation.Until
                        }
     

                        let invitationOffered : RegistrationInvitationOfferredDto   = {   
                            TenantId = invitaton.TenantId
                            Invitation =  ridto 

                        }

                        let invitationOfferedEvent = TenantStreamEvent.InvitationOfferred  invitationOffered
                        let invitationOfferedEventL = [invitationOfferedEvent] |> List.toArray

                        tenantEventList <- Array.append tenantEventList invitationOfferedEventL




                | TenantProvisionedEvent.InvitationWithdrawn invitation ->

        

                        let ridto : RegistrationInvitationDto = {
                            RegistrationInvitationId = invitation.Invitation.RegistrationInvitationId
                            Description = invitation.Invitation.Description
                            TenantId = invitation.TenantId
                            StartingOn = invitation.Invitation.StartingOn
                            Until = invitation.Invitation.Until
                        }
                      
                        let registrationInvitationWithdrawnedDto : RegistrationInvitationWithdrawnDto   = {   
                            TenantId =  invitation.TenantId
                            Invitation =  ridto

                        }

                        let invitationWithdrawn = InvitationWithdrawn registrationInvitationWithdrawnedDto
                        let invitationWithdrawnL = [invitationWithdrawn] |> List.toArray


                        tenantEventList <- Array.append tenantEventList invitationWithdrawnL


            
                   
                | ProvisionAcknowledgementSent aProvisionAcknowledgementSent ->

                    printfn "aProvisionAcknowledgementSent = %A " aProvisionAcknowledgementSent

        )|>ignore


        let tId:string = tenantId
        let rId:string = roleId
        let uId:string = userId
               

        (tId, tenantEventList , rId, roleEventList , uId, userEventList)
    




    let handleProvisionTenant (aCommand:ProvisionTenantCommand) = 

        let data = aCommand.Data

        let ouput = data |> provisionTenantWorflow 
 
        match ouput with 
        | Ok tenantProvisionedEventList ->                   
            let tenantId, tenantEventList, roleId, 
                roleEventList, userId, userEventList
                 = allTenantAggregateEvents tenantProvisionedEventList

            let saveTenantStream = async {
                let events =  tenantEventList |> Array.map toSequence
                let r = recursivePersistEventsStream tenantId -1L events
                return r
                }

            let saveUserStream = async {
                let userStreamId = userId |> concatUserStreamId  
                let events =  userEventList |> Array.map toSequence
                let r = recursivePersistEventsStream userStreamId -1L events
                return r
                } 
                   
            let saveRoleStream = async {
                let roleStreamId =  roleId |> concatRoleStreamId
                let events =  roleEventList |> Array.map toSequence  
                let r = recursivePersistEventsStream roleStreamId -1L events
                return r
                }
     
            saveTenantStream |> Async.RunSynchronously
            saveUserStream |> Async.RunSynchronously
            saveRoleStream |> Async.RunSynchronously
                
            Ok tenantProvisionedEventList

        | Error error ->
            Error error















module OfferRegistrationInvitationCommand = 


 

    let handleOfferRegistrationInvitation (aCommad:OfferRegistrationInvitationCommand) = 

        let aCommandData = aCommad.Data

        let unvalidatedRegistrationInvitationDescription = {
            TenantId = aCommandData.TenantId
            Description =  aCommandData.Description 
            }

        
            
        let rsOfferRegistrationInvitationWorkflow = result {  
            
            let strTenantId =  unvalidatedRegistrationInvitationDescription.TenantId |> concatTenantStreamId 
            let! tenantStreamId, tenantDto, lastEventNumber = strTenantId |> loadTenantWithId    


            let! rsTenant = result {      
                let! tenantFound =  tenantDto |> DbHelpers.fromDbDtoToTenant 
                let rs = unvalidatedRegistrationInvitationDescription |> offerRegistrationInvitationWorkflow tenantFound 
                return rs 
                }    
            return (rsTenant, tenantStreamId, lastEventNumber)
            }

        match rsOfferRegistrationInvitationWorkflow with  
        | Ok en ->
            match en with  
            | Ok ev, tenantStreamId, lastEventNumber -> 
                let saveOfferRegistrationInvitationEvent = async {

                        let regInvOffDto:RegistrationInvitationOfferredDto = {
                            TenantId = ev.Tenant.TenantId
                            Invitation =  ev.RegistrationInvitation
                            }
  
                        let registrstionInvitationOfferredEvent =  regInvOffDto |> TenantStreamEvent.InvitationOfferred 
                        let registrstionInvitationOfferredEvent = registrstionInvitationOfferredEvent |> toSequence |> Array.singleton

                        let rsAppendToTenantStream = recursivePersistEventsStream tenantStreamId lastEventNumber registrstionInvitationOfferredEvent
                     
                        return rsAppendToTenantStream
                        }
                saveOfferRegistrationInvitationEvent
                |> Async.RunSynchronously
                Ok ev
            | Error error, s, t ->
                Error  error
        | Error error ->
            let er = OfferRegistrationInvitationError.OfferInvitationError error
            Error er

            
                               
    


        
module WithdrawRegistrationInvitationCommand = 


 

    let handleWithdrawRegistrationInvitation (aCommad:WithdrawRegistrationInvitationCommand) = 

        let aCommandData = aCommad.Data

        let unvalidatedRegistrationInvitationIdentifier:UnvalidatedRegistrationInvitationIdentifier = {
            RegistrationInvitationId =  aCommandData.RegistrationInvitationId 
            TenantId = aCommandData.TenantId
            }

            
        let rsWithdrawRegistrationInvitationWorkflow = result {  
            
            let strTenantId =  unvalidatedRegistrationInvitationIdentifier.TenantId |> concatTenantStreamId 
            let! tenantStreamId, tenantDto, lastEventNumber = strTenantId |> loadTenantWithId    

            printfn "-----------------------------------------------------------"
            printfn "-----------------------------------------------------------"
            printfn "-----------------------------------------------------------"
            printfn "------------------- TENANT DTO %A" tenantDto
            printfn "-----------------------------------------------------------"
            printfn "-----------------------------------------------------------"
            printfn "-----------------------------------------------------------"


            let! rsTenant = result {      
                let! tenantFound =  tenantDto |> DbHelpers.fromDbDtoToTenant 
                let rs = unvalidatedRegistrationInvitationIdentifier |> withdrawRegistrationInvitationWorkflow tenantFound 
                return rs 
                }    
            return (rsTenant, tenantStreamId, lastEventNumber)
            }

        match rsWithdrawRegistrationInvitationWorkflow with  
        | Ok en ->
            match en with  
            | Ok ev, tenantStreamId, lastEventNumber ->
                let saveWithdrawRegistrationInvitationEvent = async {

                        let regInvWithDto:RegistrationInvitationWithdrawnDto = {
                            TenantId = ev.Tenant.TenantId
                            Invitation =  ev.RegistrationInvitation
                            }  
                        let registrstionInvitationOfferredEvent = regInvWithDto |> TenantStreamEvent.InvitationWithdrawn 
                        let registrstionInvitationOfferredEvent = registrstionInvitationOfferredEvent |> toSequence |> Array.singleton

                        let rsAppendToTenantStream = recursivePersistEventsStream tenantStreamId lastEventNumber registrstionInvitationOfferredEvent
                     
                        return rsAppendToTenantStream
                        }
                saveWithdrawRegistrationInvitationEvent
                |> Async.RunSynchronously
                Ok ev
            | Error error, s, t ->
                Error  error
        | Error error ->
            let er = WithdrawRegistrationInvitationError.WithdrawInvitationError error
            Error er

            
          
       


        













module DeactivateTenantActivationStatus = 

    let handleDeactivateTenantActivationStatus (aDeactivateTenantActivationStatusCommand:DeactivateTenantActivationStatusCommand) =
                                                 
        let aDeactivateTenantActivationStatusCommand = aDeactivateTenantActivationStatusCommand.Data

        let unvalidatedTenantActivationStatus:UnvalidatedTenantActivationStatus = {
            TenantId = aDeactivateTenantActivationStatusCommand.TenantId
            ActivationStatus =  aDeactivateTenantActivationStatusCommand.ActivationStatus 
            Reason = aDeactivateTenantActivationStatusCommand.Reason
            }

        
        let rsDeactivateTenantWorkflow = result {

            let! streamId, tenantDto, lastEventNumber  =  unvalidatedTenantActivationStatus.TenantId |> concatTenantStreamId  |> loadTenantWithId  

            let! rsTenant = result {
                let! tenantFound =  tenantDto |> DbHelpers.fromDbDtoToTenant 
                let rs = unvalidatedTenantActivationStatus 
                         |> deactivateTenantActivationStatusWorkflow tenantFound 
                return rs 
                }               
            return (rsTenant, streamId, lastEventNumber)
            }


        match rsDeactivateTenantWorkflow with  
        | Ok en ->
            match en with  
            | Ok ev, streamId, lastEventNumber ->
                let saveOfferRegistrationInvitationEvent = async {
                    
                    let tenantActivationStatusDeactivatedDto : TenantActivationStatusDeactivatedDto   = {   
                        Tenant = ev.Tenant 
                        ActivationStatus = ev.Tenant.ActivationStatus
                        Reason = "FIXTURE FOR NW"
                        }

                    let tenantActivationStatusDeactivatedEvent = tenantActivationStatusDeactivatedDto 
                                                                 |> ActivationStatusDeActivated 
                    let tenantActivationStatusDeactivatedEvent = tenantActivationStatusDeactivatedEvent 
                                                                 |> toSequence |> Array.singleton 

                    let rsAppendToTenantStream = recursivePersistEventsStream streamId lastEventNumber tenantActivationStatusDeactivatedEvent

                    return rsAppendToTenantStream
                    }

                saveOfferRegistrationInvitationEvent
                |> Async.RunSynchronously

                Ok ev

            | Error error, s, t ->
                Error  error


        | Error error ->
            let er = DeactivateTenantActivationStatusError.DeactivationError error
            Error er















module ReactivateTenantActivationStatus = 

    let handleReactivateTenantActivationStatus (aReactivateTenantActivationStatusCommand:ReactivateTenantActivationStatusCommand) = 

        let aReactivateTenantActivationStatusCommand = aReactivateTenantActivationStatusCommand.Data

        let unvalidatedTenantActivationStatus:UnvalidatedTenantActivationStatusData = {
            TenantId = aReactivateTenantActivationStatusCommand.TenantId
            ActivationStatus =  aReactivateTenantActivationStatusCommand.ActivationStatus 
            Reason = aReactivateTenantActivationStatusCommand.Reason
            }

                   
        let rsReactivateTenantWorkflow = result {

            let! streamId, tenantDto, lastEventNumber  =  unvalidatedTenantActivationStatus.TenantId 
                                                         |> concatTenantStreamId  |> loadTenantWithId 
            let! rsTenant = result {
                let! tenantFound =  tenantDto |> DbHelpers.fromDbDtoToTenant 
                let rs = reactivateTenantActivationStatusWorkflow tenantFound unvalidatedTenantActivationStatus
                return rs  
                }

            return rsTenant, streamId, lastEventNumber
            }

        match rsReactivateTenantWorkflow with  
        | Ok en ->
            match en with  
            | Ok ev, streamId, lastEventNumber->
                let saveTenantActivationStatusReactevatedEvent = async {

                    let tenantActivationStatusDeactivatedDto : TenantActivationStatusDeactivatedDto   = {   
                        Tenant = ev.Tenant 
                        ActivationStatus = ev.Tenant.ActivationStatus
                        Reason = "FIXTURE FOR NW"
                        }

                    let tenantActivationStatusDeactivatedEvent = tenantActivationStatusDeactivatedDto |> ActivationStatusDeActivated 
                    let tenantActivationStatusDeactivatedEvent = tenantActivationStatusDeactivatedEvent |> toSequence |> Array.singleton 
    
                    let rsAppendToTenantStream = recursivePersistEventsStream streamId  lastEventNumber tenantActivationStatusDeactivatedEvent

                    return rsAppendToTenantStream
                    }
                saveTenantActivationStatusReactevatedEvent
                |> Async.RunSynchronously
                Ok ev
            | Error error, s, t ->
                Error  error
        | Error error ->
            let er = ReactivateTenantActivationStatusError.ReactivationError error
            Error er




module ProvisionGroupCommand = 





    let handleProvisionGroup (aCommad:ProvisionGroupCommand) = 

        let aCommandData = aCommad.Data

        let unvalidatedGroup  = aCommandData

            
        let rsProvisionGroupWorkflow = result {  
            
            let strTenantId =  unvalidatedGroup.TenantId |> concatTenantStreamId 
            let! tenantStreamId, tenantDto, lastEventNumber = strTenantId |> loadTenantWithId    


            let! rsTenant = result {      
                let! tenantFound =  tenantDto |> DbHelpers.fromDbDtoToTenant 
                let rs = unvalidatedGroup |> provisionGroupWorkflow tenantFound 
                return rs 
                }    
            return (rsTenant)
            }

        match rsProvisionGroupWorkflow with  
        | Ok en ->
            match en with  
            | Ok ev -> 
                let saveProvisionGroupEvent = async {

                        let groupStreamId = ev.Group.GroupId |> concatGroupStreamId 
                        let groupProvisionedEvent =  ev.Group |> GroupStreamEvent.GroupCreated 
                        let groupProvisionedEventL = groupProvisionedEvent |> toSequence |> Array.singleton
                        let rsAppendToTenantStream = recursivePersistEventsStream groupStreamId -1L groupProvisionedEventL
                     
                        return rsAppendToTenantStream
                        }
                saveProvisionGroupEvent
                |> Async.RunSynchronously
                Ok ev
            | Error error ->
                Error  error
        | Error error ->
            let er = ProvisionGroupError.DbError error
            Error er