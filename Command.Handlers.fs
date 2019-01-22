module IdentityAndAcccess.DomainApiTypes.Handlers


open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes.ProvisionTenantWorflowImplementation
open IdentityAndAcccess.OffertRegistrationInvitationApiTypes.OffertRegistrationInvitationWorflowImplementation
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

















let fromDtoToDtoTemp (aRegInvDto:RegistrationInvitationDto):RegistrationInvitationDtoTemp = 
        let t : RegistrationInvitationDtoTemp = {
            RegistrationInvitationId = aRegInvDto.RegistrationInvitationId
            TenantId = aRegInvDto.TenantId
            Description = aRegInvDto.Description
            StartingOn = aRegInvDto.StartingOn
            Until = aRegInvDto.Until

        }
        t







let fromRegInvToDto (aRegInv:RegistrationInvitation) =
        let rs : RegistrationInvitationDtoTemp = {
            RegistrationInvitationId = aRegInv.RegistrationInvitationId |> RegistrationInvitationId.value
            Description = aRegInv.Description |> RegistrationInvitationDescription.value
            TenantId = aRegInv.TenantId |> TenantId.value
            StartingOn = aRegInv.StartingOn
            Until = aRegInv.Until
        } 
        rs










let applyTenantEvent aTenant anEvent = 

    match anEvent with 
    | TenantStreamEvent.TenantCreated t ->
      t
    | TenantStreamEvent.ActivationStatusDeActivated t ->
        {aTenant with ActivationStatus = ActivationStatusDto.Disactivated}
    
    | TenantStreamEvent.ActivationStatusReActivated t ->
        {aTenant with ActivationStatus = ActivationStatusDto.Activated}

    | TenantStreamEvent.RegistrationInvitationOfferred  t ->  
        let regInDto = t.Invitation
        {aTenant with 
            RegistrationInvitations = Array.append ([regInDto |> fromDtoToDtoTemp] |> List.toArray)   aTenant.RegistrationInvitations}
        
    | TenantStreamEvent.InvitationOfferred  t ->  
        let regInDto = t.Invitation
        {aTenant with 
            RegistrationInvitations = Array.append ([regInDto |> fromDtoToDtoTemp] |> List.toArray)   aTenant.RegistrationInvitations}     
    
    | TenantStreamEvent.InvitationWithdrawned  t ->  
        let regInDto = t.Invitation
        let filteredInvitations = aTenant.RegistrationInvitations
                                  |> Array.filter ( fun invitation -> 
                                                        not ( invitation.RegistrationInvitationId = regInDto.RegistrationInvitationId )) 
        {aTenant with RegistrationInvitations = filteredInvitations}
        


let applyUserEvent aUser anEvent = 

    match anEvent with 
    | UserStreamEvent.UserRegistered t ->
      t
    | UserStreamEvent.PasswordChanged t ->
        {aUser with Password = t |> Password.value }
    
    | UserStreamEvent.PersonalNameChanged t ->
        {aUser with FirstName = t.First |> FirstName.value ; MiddleName = t.Middle |> MiddleName.value; LastName = t.Last |> LastName.value }

   



let applyRoleEvent aRole anEvent = 

    match anEvent with 
    | RoleStreamEvent.RoleCreated t ->
      t
    

let applyGroupEvent aGroup anEvent = 

    match anEvent with 
    | GroupStreamEvent.GroupCreated t ->
      t
    


let loadTenantWithId tenantStreamId = 

    let store = EventStorePlayGround.create tenantStreamId "tcp://admin:changeit@localhost:1113" |> Async.RunSynchronously 
    let foundTenantEventStreamList,lastEventNumber,
        nextEventNumber = EventStorePlayGround.readStream<TenantStreamEvent> store tenantStreamId 0L 4095  |> Async.RunSynchronously
   
    let zeroTenantDtoStateStream =  foundTenantEventStreamList.[0]
    let tenantZeroState = match zeroTenantDtoStateStream with  TenantStreamEvent.TenantCreated t ->  t
    let tenant =  foundTenantEventStreamList |>  List.toArray |> Seq.fold applyTenantEvent tenantZeroState
    let tenantDto:TenantDto= {
        _id = tenant.TenantId
        TenantId = tenant.TenantId
        Name = tenant.Name
        Description = tenant.Description
        RegistrationInvitations = tenant.RegistrationInvitations 
        ActivationStatus = tenant.ActivationStatus 
    }
    (tenantStreamId, tenantDto, lastEventNumber)


let loadRoleWithId roleStreamId = 

    let store = EventStorePlayGround.create roleStreamId "tcp://admin:changeit@localhost:1113" |> Async.RunSynchronously 
    let foundRoleEventStreamList,lastEventNumber,
        nextEventNumber = EventStorePlayGround.readStream<RoleStreamEvent> store roleStreamId 0L 4095  |> Async.RunSynchronously
   
    let zeroRoleDtoStateStream =  foundRoleEventStreamList.[0]
    let roleZeroState = match zeroRoleDtoStateStream with  RoleStreamEvent.RoleCreated t ->  t
    let role =  foundRoleEventStreamList |>  List.toArray |> Seq.fold applyRoleEvent roleZeroState
    let roleDto:RoleDto= {
        _id = role.TenantId
        RoleId = role.RoleId
        TenantId = role.TenantId
        Name = role.Name
        Description = role.Description
        SupportNesting = role.SupportNesting
        Group = role.Group 
    }
    (roleStreamId, roleDto, lastEventNumber)


let loadUserWithId userStreamId = 

    let store = EventStorePlayGround.create userStreamId "tcp://admin:changeit@localhost:1113" |> Async.RunSynchronously 
    let foundUserEventStreamList,lastEventNumber,
        nextEventNumber = EventStorePlayGround.readStream<UserStreamEvent> store userStreamId 0L 4095  |> Async.RunSynchronously
   
    let zeroUserDtoStateStream =  foundUserEventStreamList.[0]
    let userZeroState = match zeroUserDtoStateStream with  UserStreamEvent.UserRegistered t ->  t
    let user =  foundUserEventStreamList |>  List.toArray |> Seq.fold applyUserEvent userZeroState
    let userDto:UserDto = {
        _id = user._id
        UserId = user.UserId
        TenantId = user.TenantId
        Username = user.Username
        Password = user.Password
        EnablementStatus = user.EnablementStatus
        EnablementStartDate = user.EnablementStartDate
        EnablementEndDate = user.EnablementEndDate
        EmailAddress = user.EmailAddress
        PostalAddress = user.PostalAddress
        PrimaryTel = user.PrimaryTel
        SecondaryTel = user.SecondaryTel
        FirstName = user.FirstName
        LastName = user.LastName
        MiddleName = user.MiddleName
    }
    (userStreamId, userDto, lastEventNumber)


let loadGroupWithId groupStreamId = 

    let store = EventStorePlayGround.create groupStreamId "tcp://admin:changeit@localhost:1113" |> Async.RunSynchronously 
    let foundGroupEventStreamList,lastEventNumber,
        nextEventNumber = EventStorePlayGround.readStream<GroupStreamEvent> store groupStreamId 0L 4095  |> Async.RunSynchronously
   
    let zeroGroupDtoStateStream =  foundGroupEventStreamList.[0]
    let groupZeroState = match zeroGroupDtoStateStream with  GroupStreamEvent.GroupCreated t ->  t
    let groupDto =  foundGroupEventStreamList |>  List.toArray |> Seq.fold applyGroupEvent groupZeroState

    (groupStreamId, groupDto, lastEventNumber)







let concatStreamId (p1:string) (p2:string) = p1.Trim() + p2.Trim() 
let concatTenantStreamId = concatStreamId "TENANT_With_ID_=_"
let concatRoleStreamId = concatStreamId "ROLE_With_ID_=_"
let concatUserStreamId = concatStreamId "USER_With_ID_=_"





let toSequence = fun x -> Seq.init 1 (fun _ ->  x)



let rec recursivePersistEventsStream streamId (position:int64) (eventsToPersist:seq<'T> array) = 

    let store = EventStorePlayGround.create streamId "tcp://admin:changeit@localhost:1113" |> Async.RunSynchronously 


    let eventsToPersist = eventsToPersist |> Array.toList

    match eventsToPersist with
    | [] -> ()
    | head::tail -> 

        let rsAppendToTenantStream = EventStorePlayGround.appendToStream store streamId  position head
       
        rsAppendToTenantStream 
        |> Async.RunSynchronously

        let nextStreamPostion = position + 1L
        let remainingEvents = tail |> List.toArray


        recursivePersistEventsStream streamId nextStreamPostion remainingEvents












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
                | TenantProvisionCreated aTenantProvisionCreated ->  


                    let tenantProvisioned = aTenantProvisionCreated.TenantProvisioned |> DbHelpers.fromTenantDomainToDto
                    let roleProvisioned = aTenantProvisionCreated.RoleProvisioned |> DbHelpers.fromRoleDomainToDto
                    let userRegistered = aTenantProvisionCreated.UserRegistered |> DbHelpers.fromUserDomainToDto

                    tenantId <-  concatTenantStreamId tenantProvisioned.TenantId
                    roleId <- concatRoleStreamId roleProvisioned.RoleId
                    userId <- concatUserStreamId userRegistered.UserId



                

                    let tenantProvisionedEventDto : TenantCreatedDto   = {   
                        _id = tenantProvisioned.TenantId
                        TenantId =  tenantProvisioned.TenantId 
                        Name = tenantProvisioned.Name 
                        Description = tenantProvisioned.Description 
                        RegistrationInvitations = tenantProvisioned.RegistrationInvitations
                        ActivationStatus = tenantProvisioned.ActivationStatus
                    }

                    let tenantCreatedEvent = TenantStreamEvent.TenantCreated tenantProvisionedEventDto
                    let tenantCreatedEventL = tenantCreatedEvent |> List.singleton |> List.toArray

                    tenantEventList <- Array.append tenantEventList tenantCreatedEventL



                       
        
                    let roleProvisionedEventDto : RoleProvisionedEventDto   = {   
                        _id = roleProvisioned.RoleId 
                        RoleId =  roleProvisioned.RoleId 
                        TenantId =  roleProvisioned.TenantId 
                        Name = roleProvisioned.Name 
                        Description = roleProvisioned.Description 
                        SupportNesting = roleProvisioned.SupportNesting
                        Group = roleProvisioned.Group
                    }


                    let roleCreatedEvent = RoleCreated roleProvisionedEventDto
                    let roleCreatedEventL = [roleCreatedEvent] |> List.toArray

                    roleEventList <- Array.append roleEventList roleCreatedEventL


                    
                    let userRegisteredEventDto : UserRegisteredEventDto   = {   
                        _id = userRegistered.UserId 
                        UserId =  userRegistered.UserId 
                        TenantId =  userRegistered.TenantId 
                        Username = userRegistered.Username
                        Password = userRegistered.Password
                        EnablementStatus = userRegistered.EnablementStatus
                        EnablementStartDate = userRegistered.EnablementStartDate
                        EnablementEndDate = userRegistered.EnablementEndDate
                        EmailAddress = userRegistered.EmailAddress
                        PostalAddress = userRegistered.PostalAddress
                        PrimaryTel = userRegistered.PrimaryTel
                        SecondaryTel = userRegistered.SecondaryTel
                        FirstName = userRegistered.FirstName
                        LastName = userRegistered.LastName
                        MiddleName = userRegistered.MiddleName
                    }


                    let userRegisteredEvent = UserRegistered userRegisteredEventDto
                    let userRegisteredEventL = [userRegisteredEvent] |> List.toArray

      
                    userEventList <- Array.append userEventList userRegisteredEventL


                | InvitationOffered invitaton ->


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




                | InvitationWithdrawn invitation ->

        

                        let ridto : RegistrationInvitationDto = {
                            RegistrationInvitationId = invitation.Invitation.RegistrationInvitationId
                            Description = invitation.Invitation.Description
                            TenantId = invitation.TenantId
                            StartingOn = invitation.Invitation.StartingOn
                            Until = invitation.Invitation.Until
                        }
                      
                        let registrationInvitationWithdrawnedDto : RegistrationInvitationWithdrawnedDto   = {   
                            TenantId =  invitation.TenantId
                            Invitation =  ridto

                        }

                        let invitationWithdrawn = TenantStreamEvent.InvitationWithdrawned registrationInvitationWithdrawnedDto
                        let invitationWithdrawnL = [invitationWithdrawn] |> List.toArray


                        tenantEventList <- Array.append tenantEventList invitationWithdrawnL


            
                   
                | ProvisionAcknowledgementSent aProvisionAcknowledgementSent ->

                    printfn "aProvisionAcknowledgementSent = %A " aProvisionAcknowledgementSent

        )|>ignore


        let tId:string = tenantId
        let rId:string = roleId
        let uId:string = userId
               

        (tId, tenantEventList , rId, roleEventList , uId, userEventList)
    




    let handleProvisionTenant (aProvisionTenantCommand:ProvisionTenantCommand) = 


        let aProvisionTenantCommandData = aProvisionTenantCommand.Data

        let ouput = aProvisionTenantCommandData |> provisionTenantWorflow 
 

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















module OffertRegistrationInvitationCommand = 


 




    let handleOfferRegistrationInvitation (aOfferRegistrationInvitationCommand:OfferRegistrationInvitationCommand) = 

        let aOfferRegistrationInvitationCommandData = aOfferRegistrationInvitationCommand.Data

        let unvalidatedRegistrationInvitationDescription = {
            TenantId = aOfferRegistrationInvitationCommandData.TenantId
            Description =  aOfferRegistrationInvitationCommandData.Description 
            }

        let strTenantId =  unvalidatedRegistrationInvitationDescription.TenantId |> concatTenantStreamId 
        let tenantStreamId, tenantDto, lastEventNumber = strTenantId |> loadTenantWithId  
            
        let rsOfferRegistrationInvitationWorkflow = result {    
            let! rsTenant = result {      
                let! tenantFound =  tenantDto |> DbHelpers.fromDbDtoToTenant 
                let rs = unvalidatedRegistrationInvitationDescription |> offerRegistrationInvitationWorkflow tenantFound 
                return rs 
                }    
            return rsTenant
            }

        match rsOfferRegistrationInvitationWorkflow with  
        | Ok en ->
            match en with  
            | Ok ev ->
                let saveOfferRegistrationInvitationEvent = async {

                        let tenant = ev.Tenant
                        let tenantDto = tenant |> DbHelpers.fromTenantDomainToDto
  
                        let registrstionInvitationOfferredEvent = tenantDto |> RegistrationInvitationOfferred 
                        let registrstionInvitationOfferredEvent = registrstionInvitationOfferredEvent |> toSequence |> Array.singleton

                        let rsAppendToTenantStream = recursivePersistEventsStream tenantStreamId lastEventNumber registrstionInvitationOfferredEvent
                     
                        return rsAppendToTenantStream
                        }
                saveOfferRegistrationInvitationEvent
                |> Async.RunSynchronously
                Ok ev
            | Error error ->
                Error  error
        | Error error ->
            let er = OfferRegistrationInvitationError.OfferInvitationError error
            Error er

            
                               
    


        

       


        













module DeactivateTenantActivationStatus = 

    let handleDeactivateTenantActivationStatus (aDeactivateTenantActivationStatusCommand:DeactivateTenantActivationStatusCommand) =
                                                 
        let aDeactivateTenantActivationStatusCommand = aDeactivateTenantActivationStatusCommand.Data

        let unvalidatedTenantActivationStatus:UnvalidatedTenantActivationStatus = {
            TenantId = aDeactivateTenantActivationStatusCommand.TenantId
            ActivationStatus =  aDeactivateTenantActivationStatusCommand.ActivationStatus 
            Reason = aDeactivateTenantActivationStatusCommand.Reason
            }

        let streamId, tenantDto, lastEventNumber  =  unvalidatedTenantActivationStatus.TenantId 
                                                     |> concatTenantStreamId  |> loadTenantWithId         

        let rsDeactivateTenantWorkflow = result {
            let! rsTenant = result {
                let! tenantFound =  tenantDto |> DbHelpers.fromDbDtoToTenant 
                let rs = unvalidatedTenantActivationStatus 
                         |> deactivateTenantActivationStatusWorkflow tenantFound 
                return rs 
                }               
            return rsTenant
            }


        match rsDeactivateTenantWorkflow with  
        | Ok en ->
            match en with  
            | Ok ev ->
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

                    let rsAppendToTenantStream = recursivePersistEventsStream streamId  lastEventNumber tenantActivationStatusDeactivatedEvent

                    return rsAppendToTenantStream
                    }

                saveOfferRegistrationInvitationEvent
                |> Async.RunSynchronously

                Ok ev

            | Error error ->
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

        let streamId, tenantDto, lastEventNumber  =  unvalidatedTenantActivationStatus.TenantId 
                                                     |> concatTenantStreamId  |> loadTenantWithId            
        let rsReactivateTenantWorkflow = result {
            let! rsTenant = result {
                let! tenantFound =  tenantDto |> DbHelpers.fromDbDtoToTenant 
                let rs = reactivateTenantActivationStatusWorkflow tenantFound unvalidatedTenantActivationStatus
                return rs  
                }
            return rsTenant
            }

        match rsReactivateTenantWorkflow with  
        | Ok en ->
            match en with  
            | Ok ev ->
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
            | Error error ->
                Error  error
        | Error error ->
            let er = ReactivateTenantActivationStatusError.ReactivationError error
            Error er
