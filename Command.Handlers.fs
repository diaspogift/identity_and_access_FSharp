module IdentityAndAcccess.DomainApiTypes.Handlers


open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes.ProvisionTenantWorflowImplementation
open IdentityAndAcccess.Workflow.ProvisionGroupApiTypes
open IdentityAndAcccess.Workflow.ProvisionGroupApiTypes.ProvisionGroupWorflowImplementation
open IdentityAndAcccess.Workflow.ProvisionRoleApiTypes
open IdentityAndAcccess.Workflow.ProvisionRoleApiTypes.ProvisionRoleWorflowImplementation
open IdentityAndAcccess.Workflow.AddGroupToGroupApiTypes


open IdentityAndAcccess.Workflow.AddUserToGroupApiTypes
open IdentityAndAcccess.Workflow.AddUserToGroupApiTypes.AddUserToGroupWorfklowImplementation


open IdentityAndAcccess.Workflow.AddGroupToGroupApiTypes
open IdentityAndAcccess.Workflow.AddGroupToGroupApiTypes.AddGroupToGroupApiTypes

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
open IdentityAndAcccess.DomainTypes.Functions
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
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.Aggregates.Implementation
open IdentityAndAcccess.DomainServicesImplementations
open IdentityAndAcccess.DomainTypes.Functions.Dto
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Functions.Dto
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Functions.Dto

























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

                    let tenantProvisioned = aTenantProvisionCreated.TenantProvisioned |> Dto.Tenant.fromDomain
                    let roleProvisioned = aTenantProvisionCreated.RoleProvisioned |> Dto.Role.fromDomain
                    let userRegistered = aTenantProvisionCreated.UserRegistered |> Dto.User.fromDomain

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


                    let userRegisteredEvent = userRegistered |> UserRegistered
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
    




    let handleProvisionTenant (aCommand:ProvisionTenantCommand) = 

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

            
module DeactivateTenantActivationStatus = 

    type ActivationStatusChangeData = {

        Status :  Dto.ActivationStatus  
        Reason : Dto.Reason
    }

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


module ReactivateTenantActivationStatus = 

    let handleReactivateTenantActivationStatus (aReactivateTenantActivationStatusCommand:ReactivateTenantActivationStatusCommand) = 

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


module ProvisionGroupCommand = 





    let handleProvisionGroup (aCommad:ProvisionGroupCommand) = 

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


module ProvisionRoleCommand = 





    let handleProvisionRole (aCommad:ProvisionRoleCommand) = 

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


module AddUserToGroupCommand = 





    let handleAddUserToGroup (aCommad:AddUserToGroupCommand) = 

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

            return rsAddUserToGroup, groupStreamId, lastEventNumber

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


module AddGroupToGroupCommand = 





    let handleAddGroupToGroup (aCommad:AddGroupToGroupCommand) = 

        let aCommandData = aCommad.Data
            
        let rsAddGroupToGroupWorkflow = result {  

            let strGroupIdToAddTo =  aCommandData.GroupIdToAddTo |> concatGroupStreamId 
            let strGroupIdToAdd =  aCommandData.GroupIdToAdd |> concatGroupStreamId

            printfn "UUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUU"
            printfn "IN handleAddGroupToGroup and strGroupIdToAddTo = %A" strGroupIdToAddTo
            printfn "IN handleAddGroupToGroup and strGroupIdToAdd = %A" strGroupIdToAdd
            printfn "UUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUU"

            let! groupStreamIdToAddTo, groupDtoToAddTo, 
                 lastEventNumberToAddTo = strGroupIdToAddTo |> loadGroupWithId  

            printfn "JJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJ"
            printfn "IN handleAddGroupToGroup and groupStreamIdToAddTo,  = %A" groupStreamIdToAddTo
            printfn "IN handleAddGroupToGroup and groupDtoToAddTo = %A" groupDtoToAddTo
            printfn "JJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJ"
            
            let! _, groupDtoToAdd, _ = strGroupIdToAdd |> loadGroupWithId    
           

            let! rsAddGroupToGroup = result {

                let grpDtoToAddTo = groupDtoToAddTo |> Dto.Group.Standard
                let grpDtoToAdd = groupDtoToAdd |> Dto.Group.Standard

                let! groupDomainToAddTo  =  grpDtoToAddTo |> Dto.Group.toDomain
                let! groupDomainToAdd =  grpDtoToAdd |> Dto.Group.toDomain


                printfn "TTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT"
                printfn "IN handleAddGroupToGroup and groupDomainToAddTo,  = %A" groupStreamIdToAddTo
                printfn "IN handleAddGroupToGroup and groupDomainToAdd = %A" groupDomainToAdd
                printfn "TTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT"

                let rs = addGroupToGroupWorkflow groupDomainToAddTo groupDomainToAdd
                return rs 
                }  

            return rsAddGroupToGroup, groupStreamIdToAddTo, lastEventNumberToAddTo

            }

        match rsAddGroupToGroupWorkflow with  
        | Ok en ->
            match en with  
            | Ok ev, groupStreamIdMemberWasToAddTo, lastEventNumber -> 
                let saveGroupAddedToGroupEvent = async {
                    let groupAddedToGroupEvent =  ev.GroupMemberAdded |> GroupStreamEvent.GroupAddedToGroup 
                    let groupAddedToGroupEventL =  groupAddedToGroupEvent |> toSequence |> Array.singleton
                    

                    let rsAppendToTenantStream = recursivePersistEventsStream groupStreamIdMemberWasToAddTo lastEventNumber groupAddedToGroupEventL
                 
                    return rsAppendToTenantStream
                    }      

                saveGroupAddedToGroupEvent
                |> Async.RunSynchronously
                Ok ev
            | Error error, s, t ->
                Error  error
        | Error error ->
            let er = AddGroupToGroupError.DbError error
            Error er
