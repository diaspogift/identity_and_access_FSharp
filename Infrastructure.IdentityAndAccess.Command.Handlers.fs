module IdentityAndAcccess.Commamds.Handlers


open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes.ProvisionTenantWorflowImplementation
open IdentityAndAcccess.Workflow.ProvisionRoleApiTypes
open IdentityAndAcccess.Workflow.ProvisionRoleApiTypes.ProvisionRoleWorflowImplementation
open IdentityAndAcccess.Workflow.AddGroupToGroupApiTypes
open IdentityAndAcccess.Workflow.AddGroupToGroupApiTypes.AddGroupToGroupWorkflowImplementation
open IdentityAndAcccess.Workflow.AddUserToGroupApiTypes
open IdentityAndAcccess.Workflow.AddUserToGroupApiTypes.AddUserToGroupWorfklowImplementation
open IdentityAndAcccess.Workflow.ProvisionGroupApiTypes
open IdentityAndAcccess.Workflow.ProvisionGroupApiTypes.ProvisionGroupWorflowImplementation
open IdentityAndAcccess.Workflow.OffertRegistrationInvitationApiTypes
open IdentityAndAcccess.Workflow.OffertRegistrationInvitationApiTypes.OffertRegistrationInvitationWorflowImplementation
open IdentityAndAcccess.Workflow.WithdrawRegistrationInvitationApiTypes
open IdentityAndAcccess.Workflow.WithdrawRegistrationInvitationApiTypes.WithdrawRegistrationInvitationWorflowImplementation
open IdentityAndAcccess.Workflow.DeactivateTenantActivationStatusApiTypes
open IdentityAndAcccess.Workflow.DeactivateTenantActivationStatusApiTypes.DeactivateTenantActivationStatusWorflowImplementation
open IdentityAndAcccess.Workflow.ReactivateTenantActivationStatusApiTypes
open IdentityAndAcccess.Workflow.ReactivateTenantActivationStatusApiTypes.ReactivateTenantActivationStatusWorflowImplementation


open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainTypes.Functions
open FSharp.Data.Sql

open IdentityAndAcccess.DomainTypes.Tenant

open IdentityAndAcccess.Workflow.OffertRegistrationInvitationApiTypes
open IdentityAndAcccess.EventStorePlayGround.Implementation
open IdentityAndAcccess.EventStorePlayGround.Implementation.EventStorePlayGround




open System
open FSharp.Data.Sql
open System.Collections.Generic
open IdentityAndAcccess.DomainTypes.Functions
open System.Collections.Generic
open FSharp.Data.Sql
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.Functions.Dto
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Functions.Dto
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Functions.Dto
open System.Collections.Generic







//Defining the comand types
type Command<'data> = {
    Data : 'data;
    TimeStamp : DateTime;
    UserId : string;
}




type ProvisionTenantCommand =
        Command<UnvalidatedTenantProvision> 

    
type OfferRegistrationInvitationCommand =
        Command<UnvalidatedRegistrationInvitationDescription> 


type WithdrawRegistrationInvitationCommand =
        Command<UnvalidatedRegistrationInvitationIdentifier> 


type ReactivateTenantActivationStatusCommand =
        Command<UnvalidatedTenantActivationStatusData> 


type DeactivateTenantActivationStatusCommand =
        Command<UnvalidatedTenantActivationStatus> 


type ProvisionGroupCommand =
        Command<UnvalidatedGroup> 


type ProvisionRoleCommand =
        Command<UnvalidatedRole>   


type AddUserToGroupCommand =
        Command<UnvalidatedGroupAndUserId> 


type AddGroupToGroupCommand =
        Command<UnvalidatedGroupIds>        





    


    

 












module Command =







    module ProvisionTenant = 

         // Helpers 
        let toCommand (up:UnvalidatedTenantProvision) : ProvisionTenantCommand =
            let provisionTenantCommand : ProvisionTenantCommand = {
                Data = up
                TimeStamp = DateTime.Now
                UserId = "Felicien"
                }
            provisionTenantCommand
      
        let allTenantAggregateEvents 
            (tenantProvisionedEventLis) 
            : (string * TenantStreamEvent [] * string * RoleStreamEvent [] * string * UserStreamEvent []) = 

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

                        let tenantProvisioned = aTenantProvisionCreated.TenantProvisioned 
                        let roleProvisioned = aTenantProvisionCreated.RoleProvisioned 
                        let userRegistered = aTenantProvisionCreated.UserRegistered 

                        tenantId <-   tenantProvisioned.TenantId
                        roleId <-  roleProvisioned.RoleId
                        userId <-  userRegistered.UserId

                        let tc : Dto.TenantCreated = {
                                TenantId = tenantProvisioned.TenantId
                                Tenant  = tenantProvisioned
                                }
                        
                        let tenantCreatedEvent = tc |> TenantStreamEvent.TenantCreated 
                        let tenantCreatedEventL = tenantCreatedEvent |> List.singleton |> List.toArray


                        tenantEventList <- Array.append tenantEventList tenantCreatedEventL
               

                        let roleCreatedEvent = roleProvisioned |> RoleCreated 
                        let roleCreatedEventL = [roleCreatedEvent] |> List.toArray


                        roleEventList <- Array.append roleEventList roleCreatedEventL


                        let userRegisteredEvent = userRegistered |> UserStreamEvent.UserRegistered
                        let userRegisteredEventL = [userRegisteredEvent] |> List.toArray

          
                        userEventList <- Array.append userEventList userRegisteredEventL


                    | TenantProvisionedEvent.InvitationOffered invitaton ->

                        let invitationOffered : Dto.OfferredRegistrationInvitation  = { 
                            TenantId = invitaton.TenantId  
                            OfferredInvitation = invitaton.Invitation
                        }

                        let invitationOfferedEvent = TenantStreamEvent.InvitationOfferred  invitationOffered
                        let invitationOfferedEventL = [invitationOfferedEvent] |> List.toArray

                        tenantEventList <- Array.append tenantEventList invitationOfferedEventL

                    | TenantProvisionedEvent.UserAssignedToRole assigment ->

                        let assigment : Dto.UserAssignedToRoleEvent  = { 
                            RoleId = assigment.RoleId  
                            UserId = assigment.UserId
                            AssignedUser = assigment.AssignedUser
                        }

                        let assigment = assigment |> RoleStreamEvent.UserAssignedToRole
                        let assigmentL = [assigment] |> List.toArray

                        roleEventList <- Array.append roleEventList assigmentL
                    
                    | TenantProvisionedEvent.UserUnAssignedFromRole assigment ->

                        let assigment : Dto.UserAssignedToRoleEvent  = { 
                            RoleId = assigment.RoleId  
                            UserId = assigment.UserId
                            AssignedUser = assigment.AssignedUser
                        }

                        let assigment = assigment |> RoleStreamEvent.UserAssignedToRole
                        let assigmentL = [assigment] |> List.toArray
                        
                        failwith "NOT IMPLEMENTED YET"

                        //roleEventList <- Array.append roleEventList assigmentL

                    | TenantProvisionedEvent.InvitationWithdrawn invitation ->
                          
                        let registrationInvitationWithdrawnedDto : Dto.WithnrawnRegistrationInvitation  = {   
                            TenantId = invitation.TenantId
                            WithdrawnInvitation = invitation.Invitation
                            }

                        let invitationWithdrawn = TenantStreamEvent.InvitationWithdrawn registrationInvitationWithdrawnedDto
                        let invitationWithdrawnL = [invitationWithdrawn] |> List.toArray

                        tenantEventList <- Array.append tenantEventList invitationWithdrawnL
                       
                    | ProvisionAcknowledgementSent aProvisionAcknowledgementSent ->

                        printfn "aProvisionAcknowledgementSent = %A " aProvisionAcknowledgementSent

            )|>ignore


            let tId:string = tenantId
            let rId:string = roleId
            let uId:string = userId
                   

            (tId, tenantEventList , rId, roleEventList , uId, userEventList)
        

        let allTenantAggregateDtoEvents 

            (tenantProvisionedEventLis) 
            : (string * TenantStreamEvent [] * string * RoleStreamEvent [] * string * UserStreamEvent []) = 

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

                        let tenantProvisioned = aTenantProvisionCreated.TenantProvisioned 
                        let roleProvisioned = aTenantProvisionCreated.RoleProvisioned 
                        let userRegistered = aTenantProvisionCreated.UserRegistered 

                        tenantId <-   tenantProvisioned.TenantId
                        roleId <-  roleProvisioned.RoleId
                        userId <-  userRegistered.UserId

                        let tc : Dto.TenantCreated = {
                                TenantId = tenantProvisioned.TenantId
                                Tenant  = tenantProvisioned
                                }
                        
                        let tenantCreatedEvent = tc |> TenantStreamEvent.TenantCreated 
                        let tenantCreatedEventL = tenantCreatedEvent |> List.singleton |> List.toArray


                        tenantEventList <- Array.append tenantEventList tenantCreatedEventL
               

                        let roleCreatedEvent = roleProvisioned |> RoleCreated 
                        let roleCreatedEventL = [roleCreatedEvent] |> List.toArray


                        roleEventList <- Array.append roleEventList roleCreatedEventL


                        let userRegisteredEvent = userRegistered |> UserStreamEvent.UserRegistered
                        let userRegisteredEventL = [userRegisteredEvent] |> List.toArray

          
                        userEventList <- Array.append userEventList userRegisteredEventL


                    | TenantProvisionedEvent.InvitationOffered invitaton ->

                        let invitationOffered : Dto.OfferredRegistrationInvitation  = { 
                            TenantId = invitaton.TenantId  
                            OfferredInvitation = invitaton.Invitation
                        }

                        let invitationOfferedEvent = TenantStreamEvent.InvitationOfferred  invitationOffered
                        let invitationOfferedEventL = [invitationOfferedEvent] |> List.toArray

                        tenantEventList <- Array.append tenantEventList invitationOfferedEventL

                    | TenantProvisionedEvent.UserAssignedToRole assignment ->

                        let assignmen : Dto.UserAssignedToRoleEvent  = { 
                            RoleId = assignment.RoleId
                            UserId = assignment.UserId  
                            AssignedUser = assignment.AssignedUser
                        }

                        let assignmenEvent = assignmen |> RoleStreamEvent.UserAssignedToRole 
                        let assignmenEventL = [assignmenEvent] |> List.toArray

                        roleEventList <- Array.append roleEventList assignmenEventL

                    | TenantProvisionedEvent.UserUnAssignedFromRole assignment ->

                        let assignmen : Dto.UserAssignedToRoleEvent  = { 
                            RoleId = assignment.RoleId
                            UserId = assignment.UserId  
                            AssignedUser = assignment.AssignedUser
                        }

                        let assignmenEvent = assignmen |> RoleStreamEvent.UserAssignedToRole 
                        let assignmenEventL = [assignmenEvent] |> List.toArray

                        failwith "Wrong state"

                        //roleEventList <- Array.append roleEventList assignmenEventL

                        
                    | TenantProvisionedEvent.InvitationWithdrawn invitation ->
                          
                        let registrationInvitationWithdrawnedDto : Dto.WithnrawnRegistrationInvitation  = {   
                            TenantId = invitation.TenantId
                            WithdrawnInvitation = invitation.Invitation
                            }

                        let invitationWithdrawn = TenantStreamEvent.InvitationWithdrawn registrationInvitationWithdrawnedDto
                        let invitationWithdrawnL = [invitationWithdrawn] |> List.toArray

                        tenantEventList <- Array.append tenantEventList invitationWithdrawnL
                       
                    | ProvisionAcknowledgementSent aProvisionAcknowledgementSent ->

                        printfn "aProvisionAcknowledgementSent = %A " aProvisionAcknowledgementSent

            )|>ignore


            let tId:string = tenantId
            let rId:string = roleId
            let uId:string = userId
                   

            (tId, tenantEventList , rId, roleEventList , uId, userEventList)








        let handle(aCommand:ProvisionTenantCommand) = 

            let data = aCommand.Data
            
            let ouput = data |> provisionTenantWorflow 
     
            match ouput with 
            | Ok tenantProvisionedEventList ->                   
                let tenantId, tenantEventList, roleId, 
                    roleEventList, userId, userEventList
                     = allTenantAggregateEvents tenantProvisionedEventList

                let saveTenantStream = async {
                    let tenantStreamId = tenantId |> concatTenantStreamId 
                    let events =  tenantEventList |> Array.map toSequence
                    let r = recursivePersistEventsStream tenantStreamId -1L events
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


    module OfferInvitation = 


        let toCommand (up:UnvalidatedRegistrationInvitationDescription) : OfferRegistrationInvitationCommand =
            let command : OfferRegistrationInvitationCommand = {
                Data = up
                TimeStamp = DateTime.Now
                UserId = "Felicien"
                }
            command


        let handle (aCommad:OfferRegistrationInvitationCommand) = 

            let aCommandData = aCommad.Data

            let unvalidatedRegistrationInvitationDescription:UnvalidatedRegistrationInvitationDescription = {
                TenantId = aCommandData.TenantId
                Description =  aCommandData.Description 
                }

            let rsOfferRegistrationInvitationWorkflow = result {           
                let strTenantId =  unvalidatedRegistrationInvitationDescription.TenantId |> concatTenantStreamId 
                let! tenantStreamId, tenantDto, lastEventNumber = strTenantId |> loadTenantWithId    

                let! rsTenant = result {      
                    let! tenantFound =  tenantDto |> Dto.Tenant.toDomain
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

                            let offferredInv:Dto.OfferredRegistrationInvitation = {
                                TenantId = ev.TenantId
                                OfferredInvitation = ev.OfferredInvitation  
                            }
                            let registrstionInvitationOfferredEvent =  offferredInv |> TenantStreamEvent.InvitationOfferred 
                            let registrstionInvitationOfferredEvent = registrstionInvitationOfferredEvent |> toSequence |> Array.singleton
                            let rsAppendToTenantStream = recursivePersistEventsStream tenantStreamId lastEventNumber registrstionInvitationOfferredEvent 
                            return rsAppendToTenantStream
                            }
                    saveOfferRegistrationInvitationEvent
                    |> Async.RunSynchronously
                    Ok ev
                | Error error, _, _ ->
                    Error  error
            | Error error ->
                let er = OfferRegistrationInvitationError.OfferInvitationError error
                Error er
                
            
    module WithdrawInvitation = 


        let toCommand (uri:UnvalidatedRegistrationInvitationIdentifier) : WithdrawRegistrationInvitationCommand =
            let commd : WithdrawRegistrationInvitationCommand = {
                Data = uri
                TimeStamp = DateTime.Now
                UserId = "Felicien"
                }
            commd

        let handle (aCommad:WithdrawRegistrationInvitationCommand) = 

            let aCommandData = aCommad.Data

            let unvalidatedRegistrationInvitationIdentifier:UnvalidatedRegistrationInvitationIdentifier = {
                RegistrationInvitationId =  aCommandData.RegistrationInvitationId 
                TenantId = aCommandData.TenantId
                }

                
            let rsWithdrawRegistrationInvitationWorkflow = result {  
                
                let strTenantId =  unvalidatedRegistrationInvitationIdentifier.TenantId |> concatTenantStreamId 
                let! tenantStreamId, tenantDto, lastEventNumber = strTenantId |> loadTenantWithId    

                let! rsTenant = result {      
                    let! tenantFound =  tenantDto |> Dto.Tenant.toDomain
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

                            let withnrawnRegistrationInvitation: Dto.WithnrawnRegistrationInvitation = {
                                    TenantId = ev.TenantId
                                    WithdrawnInvitation = ev.WithdrawnInvitation
                                }
           
                            let registrstionInvitationWithdrawnEvent = withnrawnRegistrationInvitation |> TenantStreamEvent.InvitationWithdrawn 
                            let registrstionInvitationOfferredEvent = registrstionInvitationWithdrawnEvent |> toSequence |> Array.singleton
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

                
    module DeactivateTenant = 



        let toCommand (uacstat:UnvalidatedTenantActivationStatus) : DeactivateTenantActivationStatusCommand =
                printfn "-----START-----"
                printfn " UnvalidatedTenantActivationStatus =  %A " uacstat
                printfn "-----ENDS-----" 
                let cmmd : DeactivateTenantActivationStatusCommand = {
                    Data = uacstat
                    TimeStamp = DateTime.Now
                    UserId = "Felicien"
                    }
                cmmd


        type ActivationStatusChangeData = {

            Status :  Dto.ActivationStatus  
            Reason : Dto.Reason
        }

        let handle (aDeactivateTenantActivationStatusCommand:DeactivateTenantActivationStatusCommand) =
                                                     
            let aDeactivateTenantActivationStatusCommand = aDeactivateTenantActivationStatusCommand.Data

            let unvalidatedTenantActivationStatus:UnvalidatedTenantActivationStatus = {
                TenantId = aDeactivateTenantActivationStatusCommand.TenantId
                ActivationStatus =  aDeactivateTenantActivationStatusCommand.ActivationStatus 
                Reason = aDeactivateTenantActivationStatusCommand.Reason
                }

            
            let rsDeactivateTenantWorkflow = result {

                let! streamId, tenantDto, lastEventNumber  =  unvalidatedTenantActivationStatus.TenantId |> concatTenantStreamId  |> loadTenantWithId  

                let! rsTenant = result {
                    let! tenantFound =  tenantDto |> Dto.Tenant.toDomain 
                    let! reason = unvalidatedTenantActivationStatus.Reason |> Reason.create'
                    let rs = unvalidatedTenantActivationStatus 
                             |> deactivateTenantActivationStatusWorkflow tenantFound reason
                    return rs 
                    }               
                return (rsTenant, streamId, lastEventNumber)
                }


            match rsDeactivateTenantWorkflow with  
            | Ok en ->
                match en with  
                | Ok ev, streamId, lastEventNumber ->
                    let saveActivationStatusDeactivatedEvent = async {

                        let tenantActivationStatusDeactivatedEvent: Dto.ActivationStatusAndReason = {
                            TenantId = ev.TenantId 
                            Status = ev.Status
                            Reason = ev.Reason
                        }

                        let tenantActivationStatusDeactivatedEvent = tenantActivationStatusDeactivatedEvent |> TenantStreamEvent.ActivationStatusDeActivated 
                        let tenantActivationStatusDeactivatedEvent = tenantActivationStatusDeactivatedEvent 
                                                                     |> toSequence |> Array.singleton 
                        let rsAppendToTenantStream = recursivePersistEventsStream streamId lastEventNumber tenantActivationStatusDeactivatedEvent
                        return rsAppendToTenantStream
                        }

                    saveActivationStatusDeactivatedEvent
                    |> Async.RunSynchronously

                    Ok ev

                | Error error, s, t ->
                    Error  error


            | Error error ->
                let er = DeactivateTenantActivationStatusError.DeactivationError error
                Error er


    module ReactivateTenant = 



        let toCommand (uacstatd:UnvalidatedTenantActivationStatusData) : ReactivateTenantActivationStatusCommand = 
            let cmmd : ReactivateTenantActivationStatusCommand = {
                Data = uacstatd
                TimeStamp = DateTime.Now
                UserId = "Felicien"
                }
            cmmd


        let handle (aReactivateTenantActivationStatusCommand:ReactivateTenantActivationStatusCommand) = 

            let aReactivateTenantActivationStatusCommand = aReactivateTenantActivationStatusCommand.Data

            let unvalidatedTenantActivationStatus:UnvalidatedTenantActivationStatusData = {
                TenantId = aReactivateTenantActivationStatusCommand.TenantId
                ActivationStatus =  aReactivateTenantActivationStatusCommand.ActivationStatus 
                Reason = aReactivateTenantActivationStatusCommand.Reason
                }

                       
            let rsReactivateTenantWorkflow = result {

                let! streamId, tenantDto, lastEventNumber  =  
                        unvalidatedTenantActivationStatus.TenantId 
                        |> concatTenantStreamId  
                        |> loadTenantWithId 


                let! rsTenant = result {
                    let! tenantFound =  tenantDto |> Dto.Tenant.toDomain 
                    let! reason = unvalidatedTenantActivationStatus.Reason |> Reason.create'
                    let rs = unvalidatedTenantActivationStatus |> reactivateTenantActivationStatusWorkflow tenantFound reason
                    return rs  
                    }

                return rsTenant, streamId, lastEventNumber
                }

            match rsReactivateTenantWorkflow with  
            | Ok en ->
                match en with  
                | Ok ev, streamId, lastEventNumber->
                    let saveTenantActivationStatusReactevatedEvent = async {
                        
                        let tenantActivationStatusReactivatedEvent: Dto.ActivationStatusAndReason = {
                            TenantId = ev.TenantId 
                            Status = ev.Status
                            Reason = ev.Reason
                        }
                        
                        let tenantActivationStatusReactivatedEvent = tenantActivationStatusReactivatedEvent |> TenantStreamEvent.ActivationStatusReActivated 
                        let tenantActivationStatusReactivatedEvent = tenantActivationStatusReactivatedEvent |> toSequence |> Array.singleton 
                        let rsAppendToTenantStream = recursivePersistEventsStream streamId  lastEventNumber tenantActivationStatusReactivatedEvent
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


    module ProvisionGroup = 


        let toCommand (ugpr:UnvalidatedGroup) : ProvisionGroupCommand =
                let cmmd : ProvisionGroupCommand = {
                    Data = ugpr
                    TimeStamp = DateTime.Now
                    UserId = "Felicien"
                    }
                cmmd



        let handle (aCommad:ProvisionGroupCommand) = 

            let aCommandData = aCommad.Data
            let unvalidatedGroup  = aCommandData
           
            let rsProvisionGroupWorkflow = result {  
                
                let strTenantId =  unvalidatedGroup.TenantId |> concatTenantStreamId 
                let! _, tenantDto, _ = strTenantId |> loadTenantWithId    

                let! rsTenant = result {      
                    let! tenantFound =  tenantDto |> Dto.Tenant.toDomain 
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


                        let groupStreamId = ev.GroupId |> concatGroupStreamId 

                        let stdrGpCreated: Dto.StandardGroupCreated = {
                             GroupId = ev.GroupId
                             TenantId = ev.TenantId
                             Group = ev.Group
                             }
                             
                        let createdGrp = Dto.GroupCreated.Standard stdrGpCreated


                        let groupProvisionedEvent =  createdGrp |> GroupStreamEvent.GroupCreated 
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


    module ProvisionRole = 



        let toCommand (ur:UnvalidatedRole) : ProvisionRoleCommand =
                let cmmd : ProvisionRoleCommand = {
                    Data = ur
                    TimeStamp = DateTime.Now
                    UserId = "Felicien"
                    }
                cmmd

        let handle (aCommad:ProvisionRoleCommand) = 

            let aCommandData = aCommad.Data
            let unvalidatedRole  = aCommandData
                
            let rsProvisionRoleWorkflow = result {  
                let strTenantId =  unvalidatedRole.TenantId |> concatTenantStreamId 
                let! tenantStreamId, tenantDto, lastEventNumber = strTenantId |> loadTenantWithId    

                let! rsTenant = result {      
                    let! tenantFound =  tenantDto |> Dto.Tenant.toDomain 
                    let rs = unvalidatedRole |> provisionRoleWorkflow tenantFound 
                    return rs 
                    }  

                return (rsTenant)

                }

            match rsProvisionRoleWorkflow with  
            | Ok en ->
                match en with  
                | Ok ev -> 
                    let saveProvisionRoleEvent = async {

                            let roleProvisionedEvent =  ev.Role |> RoleStreamEvent.RoleCreated 
                            let roleProvisionedEventL = roleProvisionedEvent |> toSequence |> Array.singleton
                            let rsAppendToTenantStream = recursivePersistEventsStream ev.Role.RoleId -1L roleProvisionedEventL
                         
                            return rsAppendToTenantStream
                            }
                    saveProvisionRoleEvent
                    |> Async.RunSynchronously
                    Ok ev
                | Error error ->
                    Error  error
            | Error error ->
                let er = ProvisionRoleError.DbError error
                Error er


    module AddUserToGroup = 





        let handle (aCommad:AddUserToGroupCommand) = 

            let aCommandData = aCommad.Data
                
            let rsAddUserToGroupWorkflow = result {  

                let strGroupId =  aCommandData.GroupId |> concatGroupStreamId 
                let strUserId =  aCommandData.UserId |> concatUserStreamId 


                let! groupStreamId, groupDto, lastEventNumber = strGroupId |> loadGroupWithId    
                let! _, userDto, _ = strUserId |> loadUserWithId    

                let! rsAddUserToGroup = result {  

                    let group = groupDto |> Dto.Group.Standard

                    let! groupDomain  =   group |> Dto.Group.toDomain
                    let! userDomain  =  userDto |> Dto.User.toDomain

                    let rs = addUserToGroupWorkflow groupDomain userDomain
                    return rs 
                    }  

                return (rsAddUserToGroup, groupStreamId, lastEventNumber)

                }

            match rsAddUserToGroupWorkflow with  
            | Ok en ->
                match en with  
                | Ok ev, groupStreamId, lastEventNumber -> 
                    let saveUserAddedToGroupEvent = async {
                        let userAddedToGroupEvent =  ev.GroupMemberAdded |> GroupStreamEvent.UserAddedToGroup 
                        let userAddedToGroupEventL =  userAddedToGroupEvent |> toSequence |> Array.singleton
                        let rsAppendToTenantStream = recursivePersistEventsStream groupStreamId lastEventNumber userAddedToGroupEventL
                        return rsAppendToTenantStream
                        }      

                    saveUserAddedToGroupEvent
                    |> Async.RunSynchronously
                    Ok ev
                | Error error, s, t ->
                    Error  error
            | Error error ->
                let er = AddUserToGroupError.DbError error
                Error er


    module AddGroupToGroup = 




        let toCommand (ur:UnvalidatedGroupIds) : AddGroupToGroupCommand =
                let cmmd : AddGroupToGroupCommand = {
                    Data = ur
                    TimeStamp = DateTime.Now
                    UserId = "Felicien"
                    }
                cmmd




        let handle (aCommad:AddGroupToGroupCommand) = 

            let aCommandData = aCommad.Data
                
            let rsAddGroupToGroupWorkflow = result {  

                let strGroupIdToAddTo =  aCommandData.GroupIdToAddTo |> concatGroupStreamId 
                let strGroupIdToAdd =  aCommandData.GroupIdToAdd |> concatGroupStreamId

                let! groupStreamIdToAddTo, groupDtoToAddTo, lastEventNumberToAddTo = strGroupIdToAddTo |> loadGroupWithId                 
                let! groupStreamIdToAdd , groupDtoToAdd, lastEventNumberToAdd = strGroupIdToAdd |> loadGroupWithId    
               
                printfn ""
                printfn ""
                printfn ""
                printfn "vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv"
                printfn "IN THE HANDLER AND groupDtoToAddTo from loadGroupWithId with id (%A) = %A" strGroupIdToAddTo groupDtoToAddTo 
                printfn "vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv"
                printfn "IN THE HANDLER AND groupDtoToAdd from loadGroupWithId with id (%A) = %A" strGroupIdToAdd groupDtoToAdd 
                printfn "vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv"
                printfn ""
                printfn ""
                printfn ""

                let! rsAddGroupToGroup = result {

                    let grpDtoToAddTo = groupDtoToAddTo |> Dto.Group.Standard
                    let grpDtoToAdd = groupDtoToAdd |> Dto.Group.Standard

                    let! groupDomainToAddTo  =  grpDtoToAddTo |> Dto.Group.toDomain
                    let! groupDomainToAdd =  grpDtoToAdd |> Dto.Group.toDomain

                    let rs = addGroupToGroupWorkflow groupDomainToAddTo groupDomainToAdd
                    return rs 
                    }  

                return (rsAddGroupToGroup, groupStreamIdToAddTo, lastEventNumberToAddTo, lastEventNumberToAdd)

                }

            match rsAddGroupToGroupWorkflow with  
            | Ok en ->
                match en with  
                | Ok events, groupStreamIdMemberWasToAddTo, lastEventNumber, lastEventNumberToAdd-> 
                    let saveBothGroupEvents = async {
                        events 
                        |> List.iter 
                            (fun event ->

                                match event with 
                                | GroupAddedToGroupEvent.MemberAdded memberAdded -> 

                                    let group1StreamId = memberAdded.GroupId |> concatGroupStreamId 
                                    let group1Evt = memberAdded |> GroupStreamEvent.GroupAddedToGroup
                                    let saveGroup1Event = async {
                                        recursivePersistEventsStream group1StreamId lastEventNumber (group1Evt |> toSequence |> Array.singleton)
                                        }
                                    
                                    saveGroup1Event 
                                    |> Async.RunSynchronously
                                     
                                | GroupAddedToGroupEvent.MemberInAdded memberInAdded ->

                                    let group2StreamId = memberInAdded.GroupId |> concatGroupStreamId 
                                    let group1Evt = memberInAdded |> GroupStreamEvent.GroupInAddedToGroup

                                    let saveGroup2Event = async {
                                        recursivePersistEventsStream group2StreamId lastEventNumberToAdd (group1Evt |> toSequence |> Array.singleton)
                                        }
                                    
                                    saveGroup2Event 
                                    |> Async.RunSynchronously
                                    )

                        return ()
                        } 

                    saveBothGroupEvents 
                    |> Async.RunSynchronously
                    Ok events
                | Error error, s, t, i ->
                    Error  error
            | Error error ->
                let er = AddGroupToGroupError.DbError error
                Error er
