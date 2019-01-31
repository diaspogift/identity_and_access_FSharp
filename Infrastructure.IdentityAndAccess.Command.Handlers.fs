module IdentityAndAcccess.Commamds.Handlers


open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes.ProvisionTenantWorflowImplementation
open IdentityAndAcccess.Workflow.ProvisionRoleApiTypes
open IdentityAndAcccess.Workflow.ProvisionRoleApiTypes.ProvisionRoleWorflowImplementation
open IdentityAndAcccess.Workflow.AssignUserToRoleApiTypes
open IdentityAndAcccess.Workflow.AssignUserToRoleApiTypes.AssignUserToRoleWorfklowImplementation
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
open IdentityAndAcccess.Workflow.AssignUserToRoleApiTypes
open System.Text.RegularExpressions







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
        Command<UnvalidatedTenantDeactivationStatus> 


type ProvisionGroupCommand =
        Command<UnvalidatedGroup> 


type ProvisionRoleCommand =
        Command<UnvalidatedRole>   


type AddUserToRoleCommand =
        Command<UnvalidatedRoleAndUserId> 


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
                    let tenantStreamId = tenantId |> toTenantStreamId 
                    let events =  tenantEventList |> Array.map toSequence
                    let r = recursivePersistEventsStream tenantStreamId -1L events
                    return r
                    }

                let saveUserStream = async {
                    let userStreamId = userId |> toUserStreamId  
                    let events =  userEventList |> Array.map toSequence
                    let r = recursivePersistEventsStream userStreamId -1L events
                    return r
                    } 
                       
                let saveRoleStream = async {
                    let roleStreamId =  roleId |> toRoleStreamId
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


            let workflowResult = result {

                let! loadInputs = result {

                    let aCommandData = aCommad.Data

                    let unvalidatedRegistrationInvitationDescription:UnvalidatedRegistrationInvitationDescription = {
                        TenantId = aCommandData.TenantId
                        Description =  aCommandData.Description 
                        }
                    let strTenantId =  unvalidatedRegistrationInvitationDescription.TenantId |> toTenantStreamId 
                    let!  tenantAggregate = strTenantId |> loadTenantWithId |> Result.mapError OfferRegistrationInvitationError.DbError

                    let tenantStreamId = tenantAggregate.StreamId
                    let  dtoTenant = tenantAggregate.Data
                    let tenantAggrLastEventNum = tenantAggregate.LastEventNum 


                    let! domainTenant = dtoTenant |> Tenant.toDomain |> Result.mapError OfferRegistrationInvitationError.ValidationError

                    return (tenantStreamId, domainTenant, tenantAggrLastEventNum, unvalidatedRegistrationInvitationDescription)
                    }

                let tenantStreamId,
                    domainTenant, 
                    tenantAggrLastEventNum,
                    unvalidatedRegistrationInvitationDescription = loadInputs 
                
                let! rsWorkflowCall = result {
                                           
                    let! rs = unvalidatedRegistrationInvitationDescription |> offerRegistrationInvitationWorkflow domainTenant 

                    return rs

                    }

                let! persistedWorflowEvents = result {
                    
                    let eventDto:Dto.OfferredRegistrationInvitation = {
                        TenantId = rsWorkflowCall.TenantId
                        OfferredInvitation = rsWorkflowCall.OfferredInvitation
                        }

                    let event = eventDto |> TenantStreamEvent.InvitationOfferred
                    let eventList =  event |> toSequence |> Array.singleton
                    
                    recursivePersistEventsStream tenantStreamId tenantAggrLastEventNum eventList 

                    return eventDto
                    }

                return persistedWorflowEvents  
                }
                
            workflowResult


    module WithdrawInvitation = 


        let toCommand (uri:UnvalidatedRegistrationInvitationIdentifier) : WithdrawRegistrationInvitationCommand =
            let commd : WithdrawRegistrationInvitationCommand = {
                Data = uri
                TimeStamp = DateTime.Now
                UserId = "Felicien"
                }
            commd

        let handle (aCommad:WithdrawRegistrationInvitationCommand) = 


            let workflowResult = result {

                let! loadInputs = result {

                    let aCommandData = aCommad.Data

                    let unvalidatedRegistrationInvitationIdentifier:UnvalidatedRegistrationInvitationIdentifier = {
                        RegistrationInvitationId =  aCommandData.RegistrationInvitationId 
                        TenantId = aCommandData.TenantId
                        }
                    let strTenantId =  unvalidatedRegistrationInvitationIdentifier.TenantId |> toTenantStreamId 
                    
                    let!  tenantAggregate = strTenantId |> loadTenantWithId |> Result.mapError WithdrawRegistrationInvitationError.DbError

                    let tenantStreamId = tenantAggregate.StreamId
                    let  dtoTenant = tenantAggregate.Data
                    let tenantAggrLastEventNum = tenantAggregate.LastEventNum 


                    let! domainTenant = dtoTenant |> Tenant.toDomain |> Result.mapError WithdrawRegistrationInvitationError.ValidationError

                    return (tenantStreamId, domainTenant, tenantAggrLastEventNum, unvalidatedRegistrationInvitationIdentifier)
                    }

                let tenantStreamId,
                    domainTenant, 
                    tenantAggrLastEventNum,
                    unvalidatedRegistrationInvitationIdentifier = loadInputs

                let! rsWorkflowCall = result {
                    let! rs = unvalidatedRegistrationInvitationIdentifier |> withdrawRegistrationInvitationWorkflow domainTenant 
                    return rs
                    }

                let! persistedWorflowEvents = result {

                        
                    let eventDto:Dto.WithnrawnRegistrationInvitation = {
                        TenantId = rsWorkflowCall.TenantId
                        WithdrawnInvitation = rsWorkflowCall.WithdrawnInvitation
                        }

                    let event = eventDto |> TenantStreamEvent.InvitationWithdrawn
                    let eventList =  event |> toSequence |> Array.singleton
                    
                    recursivePersistEventsStream tenantStreamId tenantAggrLastEventNum eventList 

                    return eventDto
                    }

                return persistedWorflowEvents
                }

            workflowResult
            

    module DeactivateTenant = 



        let toCommand (uacstat:UnvalidatedTenantDeactivationStatus) : DeactivateTenantActivationStatusCommand =
                printfn "-----START-----"
                printfn " UnvalidatedTenantActivationStatus =  %A " uacstat
                printfn "-----ENDS-----" 
                let cmmd : DeactivateTenantActivationStatusCommand = {
                    Data = uacstat
                    TimeStamp = DateTime.Now
                    UserId = "Felicien"
                    }
                cmmd


        let handle (aDeactivateTenantActivationStatusCommand:DeactivateTenantActivationStatusCommand) =


            let workflowResult = result {

                let! loadInputs = result {

                    let aDeactivateTenantActivationStatusCommand = aDeactivateTenantActivationStatusCommand.Data

                    let unvalidatedTenantActivationStatus:UnvalidatedTenantDeactivationStatus = {
                        TenantId = aDeactivateTenantActivationStatusCommand.TenantId
                        ActivationStatus =  aDeactivateTenantActivationStatusCommand.ActivationStatus 
                        Reason = aDeactivateTenantActivationStatusCommand.Reason
                        }

                    let strTenantId =  unvalidatedTenantActivationStatus.TenantId |> toTenantStreamId 
                    
                    let!  tenantAggregate = strTenantId |> loadTenantWithId |> Result.mapError DeactivateTenantActivationStatusError.DbError

                    let tenantStreamId = tenantAggregate.StreamId
                    let  dtoTenant = tenantAggregate.Data
                    let tenantAggrLastEventNum = tenantAggregate.LastEventNum 


                    let! domainTenant = dtoTenant |> Tenant.toDomain |> Result.mapError DeactivateTenantActivationStatusError.ValidationError

                    return (tenantStreamId, domainTenant, tenantAggrLastEventNum, unvalidatedTenantActivationStatus)
                    }

                let tenantStreamId,
                    domainTenant, 
                    tenantAggrLastEventNum,
                    unvalidatedTenantActivationStatus = loadInputs

                let! rsWorkflowCall = result {
                    let! reason = unvalidatedTenantActivationStatus.Reason |> Reason.create' |> Result.mapError DeactivateTenantActivationStatusError.ValidationError
                    let! rs = unvalidatedTenantActivationStatus |> deactivateTenantActivationStatusWorkflow domainTenant reason     
                    return rs           
                    }

                let! rsPersisteWorflowEvents = result {

                    let eventDto:Dto.ActivationStatusAndReason = {
                        TenantId = rsWorkflowCall.TenantId
                        Status = rsWorkflowCall.Status
                        Reason = rsWorkflowCall.Reason
                        }
                    let event = eventDto |> TenantStreamEvent.ActivationStatusDeActivated
                    let eventList =  event |> toSequence |> Array.singleton
                    
                    recursivePersistEventsStream tenantStreamId tenantAggrLastEventNum eventList 

                    return eventDto
                    }    
                
                return rsPersisteWorflowEvents
                } 
            workflowResult                                      
            

    module ReactivateTenant = 

        

        let toCommand (uacstatd:UnvalidatedTenantActivationStatusData) : ReactivateTenantActivationStatusCommand = 
            let cmmd : ReactivateTenantActivationStatusCommand = {
                Data = uacstatd
                TimeStamp = DateTime.Now
                UserId = "Felicien"
                }
            cmmd


        let handle (aReactivateTenantActivationStatusCommand:ReactivateTenantActivationStatusCommand) = 

            let workflowResult = result {

                let! loadInputs = result {

                    let aReactivateTenantActivationStatusCommand = aReactivateTenantActivationStatusCommand.Data

                    let unvalidatedTenantActivationStatus:UnvalidatedTenantActivationStatusData = {
                        TenantId = aReactivateTenantActivationStatusCommand.TenantId
                        ActivationStatus =  aReactivateTenantActivationStatusCommand.ActivationStatus 
                        Reason = aReactivateTenantActivationStatusCommand.Reason
                        }

                    let strTenantId =  unvalidatedTenantActivationStatus.TenantId |> toTenantStreamId 
                
                    let!  tenantAggregate = strTenantId |> loadTenantWithId |> Result.mapError ReactivateTenantActivationStatusError.DbError

                    let tenantStreamId = tenantAggregate.StreamId
                    let  dtoTenant = tenantAggregate.Data
                    let tenantAggrLastEventNum = tenantAggregate.LastEventNum 

                    let! domainTenant = dtoTenant |> Tenant.toDomain |> Result.mapError ReactivateTenantActivationStatusError.ValidationError

                    return (tenantStreamId, domainTenant, tenantAggrLastEventNum, unvalidatedTenantActivationStatus)
                    }

                let tenantStreamId,
                    domainTenant, 
                    tenantAggrLastEventNum,
                    unvalidatedTenantActivationStatus = loadInputs

                let! rsWorkflowCall = result {
                    let! reason = unvalidatedTenantActivationStatus.Reason |> Reason.create' |> Result.mapError ReactivateTenantActivationStatusError.ValidationError
                    let! rs = unvalidatedTenantActivationStatus |> reactivateTenantActivationStatusWorkflow domainTenant reason     
                    return rs           
                    }

                let! rsPersisteWorflowEvents = result {

                    let eventDto:Dto.ActivationStatusAndReason = {
                        TenantId = rsWorkflowCall.TenantId
                        Status = rsWorkflowCall.Status
                        Reason = rsWorkflowCall.Reason
                        }
                    let event = eventDto |> TenantStreamEvent.ActivationStatusReActivated
                    let eventList =  event |> toSequence |> Array.singleton
                    
                    recursivePersistEventsStream tenantStreamId tenantAggrLastEventNum eventList 

                    return eventDto
                    }  

                return rsPersisteWorflowEvents
                }

            workflowResult
            

    module ProvisionGroup = 


        let toCommand (ugpr:UnvalidatedGroup) : ProvisionGroupCommand =
                let cmmd : ProvisionGroupCommand = {
                    Data = ugpr
                    TimeStamp = DateTime.Now
                    UserId = "Felicien"
                    }
                cmmd



        let handle (aCommad:ProvisionGroupCommand) = 

       
            let workflowResult = result {

                let! loadInputs = result {  

                    let aCommandData = aCommad.Data
                    let unvalidatedGroup  = aCommandData

                    let strTenantId =  unvalidatedGroup.TenantId |> toTenantStreamId 
                        
                    let!  tenantAggregate = strTenantId |> loadTenantWithId |> Result.mapError ProvisionGroupError.DbError

                    let tenantStreamId = tenantAggregate.StreamId
                    let  dtoTenant = tenantAggregate.Data
                    let tenantAggrLastEventNum = tenantAggregate.LastEventNum 

                    let! domainTenant = dtoTenant |> Tenant.toDomain |> Result.mapError ProvisionGroupError.ValidationError

                    return (tenantStreamId, domainTenant, tenantAggrLastEventNum, unvalidatedGroup)
                    }


                let _,
                        domainTenant, 
                        _,
                        unvalidatedGroup = loadInputs

                
                let! rscallWorkflow = result {  
                    return! provisionGroupWorkflow domainTenant unvalidatedGroup 
                    }

                let! rsPersisteWorflowEvents = result {

                    let eventDto:Dto.StandardGroup = rscallWorkflow.Group 


                    let stdGrpCreated:Dto.StandardGroupCreated = {
                        GroupId = eventDto.GroupId
                        TenantId = eventDto.TenantId
                        Group = eventDto
                    }

                    let event = stdGrpCreated |> Dto.GroupCreated.Standard  |> GroupStreamEvent.GroupCreated
                    let eventList =  event |> toSequence |> Array.singleton
                    let groupStreamId = toGroupStreamId eventDto.GroupId
                    
                    recursivePersistEventsStream groupStreamId -1L eventList

                    return eventDto
                    } 
                return rsPersisteWorflowEvents
                }

            workflowResult


    module ProvisionRole = 



        let toCommand (ur:UnvalidatedRole) : ProvisionRoleCommand =
                let cmmd : ProvisionRoleCommand = {
                    Data = ur
                    TimeStamp = DateTime.Now
                    UserId = "Felicien"
                    }
                cmmd

        let handle (aCommad:ProvisionRoleCommand) = 


            let workflowResult = result {

                let! loadInputs = result {  

                    let aCommandData = aCommad.Data
                    let unvalidatedRole  = aCommandData
                    let strTenantId =  unvalidatedRole.TenantId |> toTenantStreamId 
                        
                    let!  tenantAggregate = strTenantId |> loadTenantWithId |> Result.mapError ProvisionRoleError.DbError

                    let tenantStreamId = tenantAggregate.StreamId
                    let  dtoTenant = tenantAggregate.Data
                    let tenantAggrLastEventNum = tenantAggregate.LastEventNum 

                    let! domainTenant = dtoTenant |> Tenant.toDomain |> Result.mapError ProvisionRoleError.ValidationError

                    return (tenantStreamId, domainTenant, tenantAggrLastEventNum, unvalidatedRole)
                    }


                let _,
                        domainTenant, 
                        _,
                        unvalidatedRole = loadInputs

                
                let! rscallWorkflow = result {  
                    return! provisionRoleWorkflow domainTenant unvalidatedRole 
                    }

                let! rsPersisteWorflowEvents = result {

                    let eventDto:Dto.Role = rscallWorkflow.Role
                    let event = eventDto |> RoleStreamEvent.RoleCreated
                    let eventList =  event |> toSequence |> Array.singleton
                    let roleStreamId = toRoleStreamId eventDto.RoleId
                    
                    recursivePersistEventsStream roleStreamId -1L eventList

                    return eventDto
                    } 
                return rsPersisteWorflowEvents
                }

            workflowResult
            

    module AddUserToRole = 




        let handle (aCommad:AddUserToRoleCommand) = 

     
            let workflowResult = result {
                

                let! loadInputs = result {  

                    let aCommandData = aCommad.Data

                    let strRoleId =  aCommandData.RoleId |> toRoleStreamId 
                    let strUserId =  aCommandData.UserId |> toUserStreamId 

                    let! roleAggregate = strRoleId |> loadRoleWithId |> Result.mapError AssignUserToRoleError.DbError  
                    let! userAggregate = strUserId |> loadUserWithId |> Result.mapError AssignUserToRoleError.DbError  

                    let roleStreamId = roleAggregate.StreamId
                    let  roleDto = roleAggregate.Data
                    let roleAggrLastEventNum = roleAggregate.LastEventNum 

                    let userStreamId = userAggregate.StreamId
                    let userDto = userAggregate.Data
                    let userAggrLastEventNum = userAggregate.LastEventNum 

                    let! domainRole =   roleDto |> Role.toDomain |> Result.mapError AssignUserToRoleError.ValidationError  
                    let! domainUser =   userDto |> User.toDomain |> Result.mapError AssignUserToRoleError.ValidationError  

                    return (roleStreamId, roleAggrLastEventNum, domainRole , userStreamId, userAggrLastEventNum, domainUser)
                    }


                let roleStreamId,
                    roleAggrLastEventNum, 
                    domainRole, 
                    _,
                    _, 
                    domainUser = loadInputs


                let! rscallWorkflow = result {  
                    return! assignUserToRoleWorkflow domainRole domainUser  
                    }

                    
                let! persisteWorflowEvents = result {  

                    let eventDto:Dto.UserAssignedToRoleEvent = {
                        RoleId = rscallWorkflow.RoleId
                        UserId = rscallWorkflow.UserId
                        AssignedUser = rscallWorkflow.UserAssigned
                        }

                    let event = eventDto |> RoleStreamEvent.UserAssignedToRole
                    let eventList =  event |> toSequence |> Array.singleton
                    
                    recursivePersistEventsStream roleStreamId roleAggrLastEventNum eventList

                    return eventDto
                    } 


                return persisteWorflowEvents
                }

            workflowResult   
               

          

    module AddUserToGroup = 





        let handle (aCommad:AddUserToGroupCommand) = 

            let aCommandData = aCommad.Data
                
            let workflowResult = result {  

                let! loadInputs = result {

                    let groupStreamId =  aCommandData.GroupId |> toGroupStreamId 
                    let userStreamId =  aCommandData.UserId |> toUserStreamId

                    let! dtoGroupAggregate = groupStreamId |> loadGroupWithId |> Result.mapError AddUserToGroupError.DbError
                    let! dtoUserAggregate = userStreamId |> loadUserWithId |> Result.mapError AddUserToGroupError.DbError

                    let dtoGroup = dtoGroupAggregate.Data
                    let dtoUser = dtoUserAggregate.Data

                    let dGroup = dtoGroup |> Dto.Group.Standard  

                    let! domainGroup = dGroup |> Group.toDomain |> Result.mapError AddUserToGroupError.ValidationError
                    let! domainUser = dtoUser |> User.toDomain |> Result.mapError AddUserToGroupError.ValidationError
                    
                    return (groupStreamId, domainGroup, dtoGroupAggregate.LastEventNum, userStreamId, domainUser, dtoUserAggregate.LastEventNum)
                    }
                 

                let groupStreamId,
                    domainGroup, 
                    groupAggrLastEventNum, 
                    _,
                    domainUser, 
                    _ = loadInputs

                let! rsAddUserToGroup = result {  
                    return! addUserToGroupWorkflow domainGroup domainUser 
                    }  


                let! persisteWorflowEvents = result {  

                    let eventDto:Dto.MemberAddedToGroupEvent = {
                        GroupId = rsAddUserToGroup.GroupMemberAdded.MemberId
                        TenantId = rsAddUserToGroup.GroupMemberAdded.TenantId
                        MemberAdded = rsAddUserToGroup.GroupMemberAdded
                        }

                    let event = eventDto |> GroupStreamEvent.GroupAddedToGroup
                    let eventList =  event |> toSequence |> Array.singleton
                    
                    recursivePersistEventsStream groupStreamId groupAggrLastEventNum eventList

                    return eventDto
                    } 
                return persisteWorflowEvents

                }

            workflowResult

            




    module AddGroupToGroup = 




        let toCommand (ur:UnvalidatedGroupIds) : AddGroupToGroupCommand =
                let cmmd : AddGroupToGroupCommand = {
                    Data = ur
                    TimeStamp = DateTime.Now
                    UserId = "Felicien"
                    }
                cmmd




        let handle (aCommad:AddGroupToGroupCommand) = 

            

            let workflowResult = result {  

                let! loadInputs = result {

                    let aCommandData = aCommad.Data
                    let groupToAddToStreamId =  aCommandData.GroupIdToAdd |> toGroupStreamId 
                    let groupToAddStreamId =  aCommandData.GroupIdToAdd |> toGroupStreamId

                    let! dtoGroupToAddToAggregate = groupToAddToStreamId |> loadGroupWithId |> Result.mapError AddGroupToGroupError.DbError
                    let! dtoGroupToAddAggregate = groupToAddStreamId |> loadGroupWithId |> Result.mapError AddGroupToGroupError.DbError

                    let dtoGroupToAddTo = dtoGroupToAddToAggregate.Data
                    let dtoGroupToAdd= dtoGroupToAddAggregate.Data

                    let dGroupToAddTo = dtoGroupToAddTo |> Dto.Group.Standard  
                    let dGroupToAdd = dtoGroupToAdd |> Dto.Group.Standard  

                    let! domainGroupToAddTo = dGroupToAddTo |> Group.toDomain |> Result.mapError AddGroupToGroupError.ValidationError
                    let! domainGroupToAdd = dGroupToAdd |> Group.toDomain |> Result.mapError AddGroupToGroupError.ValidationError
                    
                    return (groupToAddToStreamId, domainGroupToAddTo, dtoGroupToAddToAggregate.LastEventNum, groupToAddStreamId, domainGroupToAdd, dtoGroupToAddAggregate.LastEventNum)
                    }
                 

                let groupToAddToStreamId,
                    domainGroupToAddTo, 
                    groupToAddToAggrLastEventNum, 
                    groupToAddStreamId,
                    domainGroupToAdd, 
                    groupToAddAggrLastEventNum = loadInputs

                let! rsAddUserToGroup = result {  
                    return! addGroupToGroupWorkflow domainGroupToAddTo domainGroupToAdd 
                    }  


                let! persisteWorflowEvents = result {  

                    let groupToAddToEvent = 
                        rsAddUserToGroup
                        |> List.iter (fun ev ->

                            match ev with 
                            | GroupAddedToGroupEvent.MemberAdded  memberAddedEvent ->

                                let eventDtoGroupToAddTo:Dto.MemberAddedToGroupEvent = {
                                    GroupId = memberAddedEvent.GroupId
                                    TenantId = memberAddedEvent.TenantId
                                    MemberAdded = memberAddedEvent.MemberAdded
                                    }
                                let event = eventDtoGroupToAddTo |> GroupStreamEvent.GroupAddedToGroup
                                let eventList =  event |> toSequence |> Array.singleton

                                recursivePersistEventsStream groupToAddToStreamId groupToAddToAggrLastEventNum eventList

                                
                            | GroupAddedToGroupEvent.MemberInAdded memberInAddedEvent ->
                                
                                let eventDtoGroupToAdd:Dto.MemberInAddedToGroupEvent = {
                                    GroupId = memberInAddedEvent.GroupId
                                    TenantId = memberInAddedEvent.TenantId
                                    MemberInAdded = memberInAddedEvent.MemberInAdded
                                    }
                                let event = eventDtoGroupToAdd |> GroupStreamEvent.GroupInAddedToGroup
                                let eventList =  event |> toSequence |> Array.singleton

                                recursivePersistEventsStream groupToAddStreamId groupToAddAggrLastEventNum eventList

                             )
                            

                                
                             

                        

                    return rsAddUserToGroup
                    }
 


                return persisteWorflowEvents
                }

            workflowResult
                
            (* let rsAddGroupToGroupWorkflow = result {  

                let strGroupIdToAddTo =  aCommandData.GroupIdToAddTo |> toGroupStreamId 
                let strGroupIdToAdd =  aCommandData.GroupIdToAdd |> toGroupStreamId

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

                                    let group1StreamId = memberAdded.GroupId |> toGroupStreamId 
                                    let group1Evt = memberAdded |> GroupStreamEvent.GroupAddedToGroup
                                    let saveGroup1Event = async {
                                        recursivePersistEventsStream group1StreamId lastEventNumber (group1Evt |> toSequence |> Array.singleton)
                                        }
                                    
                                    saveGroup1Event 
                                    |> Async.RunSynchronously
                                     
                                | GroupAddedToGroupEvent.MemberInAdded memberInAdded ->

                                    let group2StreamId = memberInAdded.GroupId |> toGroupStreamId 
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
                Error er *)

            
