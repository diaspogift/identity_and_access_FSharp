module IdentityAndAcccess.DomainApiTypes.Handlers


open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes.ProvisionTenantWorflowImplementation
open IdentityAndAcccess.OffertRegistrationInvitationApiTypes.OffertRegistrationInvitationWorflowImplementation
open IdentityAndAcccess.Workflow.DeactivateTenantActivationStatusApiTypes

open FSharp.Data.Sql
open IdentityAndAcccess.Workflow.OffertRegistrationInvitationApiTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainTypes.Tenant

open IdentityAndAcccess.EventStorePlayGround.Implementation


open IdentityAndAccess.DatabaseTypes

open IdentityAndAcccess.DeactivateTenantActivationStatusApiTypes.DeactivateTenantActivationStatusWorflowImplementation
open IdentityAndAcccess.ReactivateTenantActivationStatusApiTypes.ReactivateTenantActivationStatusWorflowImplementation

open IdentityAndAcccess.Workflow.ReactivateTenantActivationStatusApiTypes














module ProvisionTenant = 




    //let saveOneTenant = TenantDb.saveOneTenant 
    //let saveOneUser = UserDb.saveOneUser
    //let saveOneRole = RoleDb.saveOneRole 









    let handleProvisionTenant (aProvisionTenantCommand:ProvisionTenantCommand) = 

        //Extract the command data


        let aProvisionTenantCommandData = aProvisionTenantCommand.Data

        //Call into pure business logic


        let ouput = 
            aProvisionTenantCommandData 
            |> provisionTenantWorflow 



        //IO ad the edge base on the result / decision from the actual worflow



        match ouput with 
        | Ok tenantProvisionedEventList ->

            let firstEvent : TenantProvisionedEvent  = 
                tenantProvisionedEventList 
                |> List.head

            match firstEvent with
            | TenantProvisionCreated aTenantProvisionCreated->  


                let tenantProvisioned = aTenantProvisionCreated.TenantProvisioned |> DbHelpers.fromTenantDomainToDto
                let roleProvisioned = aTenantProvisionCreated.RoleProvisioned |> DbHelpers.fromRoleDomainToDto
                let userRegistered = aTenantProvisionCreated.UserRegistered |> DbHelpers.fromUserDomainToDto


                let saveOneTenantProvisionedEvent = async {

                    let strTenantId = tenantProvisioned.TenantId 
                    let prefix = "TENANT_With_ID_"
                    let connName = prefix + strTenantId 
                    let! store = EventStorePlayGround.create connName "tcp://admin:changeit@localhost:1113"

              

                    let tenantProvisionedEventDto : TenantCreatedDto   = {   
                        _id = tenantProvisioned.TenantId
                        TenantId =  tenantProvisioned.TenantId 
                        Name = tenantProvisioned.Name 
                        Description = tenantProvisioned.Description 
                        RegistrationInvitations = tenantProvisioned.RegistrationInvitations
                        ActivationStatus = tenantProvisioned.ActivationStatus
                    }

                    let tenantCreatedEvent = TenantCreated tenantProvisionedEventDto

                    let events = Seq.init 1 (fun _ ->  tenantCreatedEvent)
                    let tenantStreamIdPart1 = "TENANT_With_ID_=_"
                    let tenantStreamIdPart2 = tenantProvisioned.TenantId
                    let tenatstreamId = tenantStreamIdPart1 + tenantStreamIdPart2
                    let! rsAppendToTenantStream = EventStorePlayGround.appendToStream store tenatstreamId  -1L events

                    return rsAppendToTenantStream

                }

                let saveOneRoleProvisionedEvent = async {


                    let strRoleId = tenantProvisioned.TenantId 
                    let prefix = "ROLE_With_ID_"
                    let connName = prefix + strRoleId 
                    let! store = EventStorePlayGround.create connName "tcp://admin:changeit@localhost:1113"


                    let roleProvisionedEventDto : RoleProvisionedEventDto   = {   
                        _id = roleProvisioned.RoleId 
                        RoleId =  tenantProvisioned.TenantId 
                        TenantId =  roleProvisioned.TenantId 
                        Name = roleProvisioned.Name 
                        Description = roleProvisioned.Description 
                        SupportNesting = roleProvisioned.SupportNesting
                        Group = roleProvisioned.Group
                    }


                    let roleCreatedEvent = RoleCreated roleProvisionedEventDto

                    let events = Seq.init 1 (fun _ ->  roleCreatedEvent)
                    let roleStreamIdPart1 = "ROLE_With_ID_=_"
                    let roleStreamIdPart2 = roleProvisioned.RoleId
                    let roleStreamId = roleStreamIdPart1 +  roleStreamIdPart2
                    let! rsAppendToRoleStream = EventStorePlayGround.appendToStream store roleStreamId  -1L events

                    return rsAppendToRoleStream

                }

                let saveOneUserRegisteredEvent = async {


                    let strUserId = tenantProvisioned.TenantId 
                    let prefix = "USER_With_ID_"
                    let connName = prefix + strUserId 
                    let! store = EventStorePlayGround.create connName "tcp://admin:changeit@localhost:1113"


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


                    let roleCreatedEvent = UserRegistered userRegisteredEventDto

                    let events = Seq.init 1 (fun _ ->  roleCreatedEvent)
                    let userStreamIdPart1 = "USER_With_ID_=_"
                    let userStreamIdPart2 = userRegistered.UserId
                    let userStreamId = userStreamIdPart1 +  userStreamIdPart2
                    let! rsAppendToUserStream = EventStorePlayGround.appendToStream store userStreamId  -1L events

                    return rsAppendToUserStream

                }


                saveOneTenantProvisionedEvent 
                |>  Async.RunSynchronously
                saveOneRoleProvisionedEvent 
                |>  Async.RunSynchronously
                saveOneUserRegisteredEvent 
                |>  Async.RunSynchronously


                Ok tenantProvisionedEventList



            | ProvisionAcknowledgementSent aProvisionAcknowledgementSent ->

                printfn "aProvisionAcknowledgementSent = %A " aProvisionAcknowledgementSent

                Ok tenantProvisionedEventList


        | Error error ->

            Error error















module OffertRegistrationInvitationCommand = 


    //let loadTenantById = TenantDb.loadOneTenantById 
    //let saveOneTenant = TenantDb.saveOneTenant





    let fromRegInvToDto (aRegInv:RegistrationInvitation) =
                            let rs : RegistrationInvitationDtoTemp = {
                                RegistrationInvitationId = aRegInv.RegistrationInvitationId |> RegistrationInvitationId.value
                                Description = aRegInv.Description |> RegistrationInvitationDescription.value
                                TenantId = aRegInv.TenantId |> TenantId.value
                                StartingOn = aRegInv.StartingOn
                                Until = aRegInv.Until
                            } 
                            rs






    let handleOfferRegistrationInvitation 
                            (aOfferRegistrationInvitationCommand:OfferRegistrationInvitationCommand)
                            :Result<RegistrationInvitationOfferredEvent, OfferRegistrationInvitationError> = 






         //Helpers


        let aOfferRegistrationInvitationCommandData = aOfferRegistrationInvitationCommand.Data

        let fromDtoToDtoTemp (aRegInvDto:RegistrationInvitationDto):RegistrationInvitationDtoTemp = 
            let t : RegistrationInvitationDtoTemp = {
                RegistrationInvitationId = aRegInvDto.RegistrationInvitationId
                TenantId = aRegInvDto.TenantId
                Description = aRegInvDto.Description
                StartingOn = aRegInvDto.StartingOn
                Until = aRegInvDto.Until

            }
            t

        //IO at the edges



        let unvalidatedRegistrationInvitationDescription:UnvalidatedRegistrationInvitationDescription = {
            TenantId = aOfferRegistrationInvitationCommandData.TenantId
            Description =  aOfferRegistrationInvitationCommandData.Description 
            }

        

        let strTenantId = unvalidatedRegistrationInvitationDescription.TenantId 
        let prefix = "TENANT_With_ID_=_"
        let connName = prefix + strTenantId 
        let store = EventStorePlayGround.create connName "tcp://admin:changeit@localhost:1113" |> Async.RunSynchronously 

        printfn "LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL"
        printfn "STREAM ID FOR TENANT TO REPLAY = %A" connName
        printfn "LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL"



        let foundTenantEventStreamList,lastEventNumber,nextEventNumber = EventStorePlayGround.readStream<TenantStreamEvent> store connName 0L 4095  |> Async.RunSynchronously



        
        printfn "FoundTenantEventStreamList is foundTenantEventStreamList Satrt "
        printfn  "HER THE ACTUAL foundTenantEventStreamList,_,_  = %A" foundTenantEventStreamList
        printfn "FoundTenantEventStreamList is foundTenantEventStreamList End "
        printfn "LastTenatEventStream Number is  lastEventNumber Start="
        printfn "%A " lastEventNumber
        printfn "LastTenatEventStream Number is  lastEventNumber End"
        printfn "NextEventNumber nextEventNumber Start ="
        printfn "%A " nextEventNumber
        printfn "NextEventNumber nextEventNumber End"




        let apply aTenant anEvent = 
            match anEvent with 
            | TenantStreamEvent.TenantCreated t ->
              t
            | TenantStreamEvent.ActivationStatusDeActivated t ->
                {aTenant with ActivationStatus = ActivationStatusDto.Disactivated}
            | TenantStreamEvent.ActivationStatusReActivated t ->
                {aTenant with ActivationStatus = ActivationStatusDto.Activated}

            | TenantStreamEvent.RegistrationInvitationOfferred  t ->  
                let regInDto = t.Invitation
                let tt = {aTenant with RegistrationInvitations = Array.append ([regInDto |> fromDtoToDtoTemp] |> List.toArray)   aTenant.RegistrationInvitations}
                printfn "OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO"
                printfn "RETURNED TENANT %A" tt
                printfn "OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO"
                tt
                


                


        let stateZero : TenantCreatedDto = {
            _id = "5c4353b53766624bce89cf91"
            TenantId = "5c4353b53766624bce89cf91"
            Name = "Le Quattro"
            Description =  "Restauration - Mets locaux et traditionnels"
            RegistrationInvitations = [] |> List.toArray
            ActivationStatus = ActivationStatusDto.Activated
        }






        let state = foundTenantEventStreamList 
                    |> Seq.fold apply stateZero

        let tenantDto:TenantDto = {
            _id = state._id
            TenantId = state.TenantId
            Name = state.Name
            Description  = state.Description
            RegistrationInvitations = state.RegistrationInvitations
            ActivationStatus = state.ActivationStatus
            
        }
                       
        

        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "STATEEEEEE ================== %A" state
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"




        let rsOfferRegistrationInvitationWorkflow = result {
            
            let! rsTenant = result {
                
                let! tenantFound =  tenantDto |> DbHelpers.fromDbDtoToTenant 


                let rs = offerRegistrationInvitationWorkflow tenantFound unvalidatedRegistrationInvitationDescription

                printfn "ppppppppppppppppppppppppppppppppppppppp"
                printfn "HERE THE ACTUAL %A" rs
                printfn "ppppppppppppppppppppppppppppppppppppppp"
                          

                return rs 
            }

          
            return rsTenant
        }



        match rsOfferRegistrationInvitationWorkflow with  
        | Ok en ->
            match en with  
            | Ok ev ->
                let saveOfferRegistrationInvitationEvent = async {

                        let strTenantId = ev.Tenant.TenantId |> TenantId.value
                        let prefix = "TENANT_With_ID_"
                        let connName = prefix + strTenantId 
                        let! store = EventStorePlayGround.create connName "tcp://admin:changeit@localhost:1113"


                        let fromRegInvToDto (aRegInv:RegistrationInvitation) =
                            let rs : RegistrationInvitationDtoTemp = {
                                RegistrationInvitationId = aRegInv.RegistrationInvitationId |> RegistrationInvitationId.value
                                Description = aRegInv.Description |> RegistrationInvitationDescription.value
                                TenantId = aRegInv.TenantId |> TenantId.value
                                StartingOn = aRegInv.StartingOn
                                Until = aRegInv.Until
                            } 
                            rs

                        let invs =ev.Tenant.RegistrationInvitations |> List.map fromRegInvToDto|> List.toArray

                        let activationStatus = match ev.Tenant.ActivationStatus with
                                               | Activated -> ActivationStatusDto.Activated
                                               | Deactivated -> ActivationStatusDto.Disactivated




                        let tenantDto : TenantDto = {
                            _id = strTenantId 
                            TenantId =  strTenantId
                            Name =  ev.Tenant.Name |> TenantName.value
                            Description =  ev.Tenant.Description |> TenantDescription.value
                            RegistrationInvitations =  invs
                            ActivationStatus =  activationStatus
                        }


                        let registrationInvitationDto : RegistrationInvitationDto = {
                            RegistrationInvitationId = ev.RegistrationInvitation.RegistrationInvitationId |> RegistrationInvitationId.value
                            Description = ev.RegistrationInvitation.Description |> RegistrationInvitationDescription.value
                            TenantId = ev.RegistrationInvitation.TenantId |> TenantId.value
                            StartingOn = ev.RegistrationInvitation.StartingOn
                            Until = ev.RegistrationInvitation.Until
                        }
                  

                        let registrationInvitationOfferredEventDto : RegistrationInvitationOfferredDto   = {   
                            Tenant = tenantDto
                            Invitation = registrationInvitationDto
                        }

                        let registrstionInvitationOfferredEvent = RegistrationInvitationOfferred registrationInvitationOfferredEventDto

                        let events = Seq.init 1 (fun _ ->  registrstionInvitationOfferredEvent)
                        let tenantStreamIdPart1 = "TENANT_With_ID_=_"
                        let tenantStreamIdPart2 = tenantDto.TenantId
                        let tenatstreamId = tenantStreamIdPart1 + tenantStreamIdPart2
                        let! rsAppendToTenantStream = EventStorePlayGround.appendToStream store tenatstreamId  lastEventNumber events

                        
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

    let handleDeactivateTenantActivationStatus 
                            (aDeactivateTenantActivationStatusCommand:DeactivateTenantActivationStatusCommand)
                            :Result<TenantActivationStatusDeactivatedEvent, DeactivateTenantActivationStatusError> = 






         //Helpers
        let fromRegInvToDto (aRegInv:RegistrationInvitation) =
            let rs : RegistrationInvitationDtoTemp = {
                RegistrationInvitationId = aRegInv.RegistrationInvitationId |> RegistrationInvitationId.value
                Description = aRegInv.Description |> RegistrationInvitationDescription.value
                TenantId = aRegInv.TenantId |> TenantId.value
                StartingOn = aRegInv.StartingOn
                Until = aRegInv.Until
            } 
            rs
        let fromDtoToDtoTemp (aRegInvDto:RegistrationInvitationDto):RegistrationInvitationDtoTemp = 
            let t : RegistrationInvitationDtoTemp = {
                RegistrationInvitationId = aRegInvDto.RegistrationInvitationId
                TenantId = aRegInvDto.TenantId
                Description = aRegInvDto.Description
                StartingOn = aRegInvDto.StartingOn
                Until = aRegInvDto.Until

            }
            t




        //Input

        let aDeactivateTenantActivationStatusCommand = aDeactivateTenantActivationStatusCommand.Data

 

        //IO at the edges



        let unvalidatedTenantActivationStatus:UnvalidatedTenantActivationStatus = {
            TenantId = aDeactivateTenantActivationStatusCommand.TenantId
            ActivationStatus =  aDeactivateTenantActivationStatusCommand.ActivationStatus 
            Reason = aDeactivateTenantActivationStatusCommand.Reason
            }

        

        let strTenantId = unvalidatedTenantActivationStatus.TenantId 
        let prefix = "TENANT_With_ID_=_"
        let connName = prefix + strTenantId 
        let store = EventStorePlayGround.create connName "tcp://admin:changeit@localhost:1113" |> Async.RunSynchronously 

        printfn "LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL"
        printfn "STREAM ID FOR TENANT TO REPLAY = %A" connName
        printfn "LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL"



        let foundTenantEventStreamList,lastEventNumber,nextEventNumber = EventStorePlayGround.readStream<TenantStreamEvent> store connName 0L 4095  |> Async.RunSynchronously



        
        printfn "FoundTenantEventStreamList is foundTenantEventStreamList Satrt "
        printfn  "HER THE ACTUAL foundTenantEventStreamList,_,_  = %A" foundTenantEventStreamList
        printfn "FoundTenantEventStreamList is foundTenantEventStreamList End "
        printfn "LastTenatEventStream Number is  lastEventNumber Start="
        printfn "%A " lastEventNumber
        printfn "LastTenatEventStream Number is  lastEventNumber End"
        printfn "NextEventNumber nextEventNumber Start ="
        printfn "%A " nextEventNumber
        printfn "NextEventNumber nextEventNumber End"




        let apply aTenant anEvent = 


            let mutable i = 0

            match anEvent with 
            | TenantStreamEvent.TenantCreated t ->
              t
            | TenantStreamEvent.ActivationStatusDeActivated t ->
                {aTenant with ActivationStatus = ActivationStatusDto.Disactivated}
            | TenantStreamEvent.ActivationStatusReActivated t ->
                {aTenant with ActivationStatus = ActivationStatusDto.Activated}

            | TenantStreamEvent.RegistrationInvitationOfferred  t ->  
                let regInDto = t.Invitation
                let tt = {aTenant with RegistrationInvitations = Array.append ([regInDto |> fromDtoToDtoTemp] |> List.toArray)   aTenant.RegistrationInvitations}
                printfn "OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO"
                printfn "RETURNED RegistrationInvitationOfferred NUM = %A" (i <- i + 1)
                printfn "OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO"
                tt
                


                


        let stateZero : TenantCreatedDto = {
            _id = "5c4353b53766624bce89cf91"
            TenantId = "5c4353b53766624bce89cf91"
            Name = "Le Quattro"
            Description =  "Restauration - Mets locaux et traditionnels"
            RegistrationInvitations = [] |> List.toArray
            ActivationStatus = ActivationStatusDto.Activated
        }






        let state = foundTenantEventStreamList 
                    |> Seq.fold apply stateZero

        let tenantDto:TenantDto = {
            _id = state._id
            TenantId = state.TenantId
            Name = state.Name
            Description  = state.Description
            RegistrationInvitations = state.RegistrationInvitations
            ActivationStatus = state.ActivationStatus
            
        }
                       
        

        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "STATEEEEEE ================== %A" state
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"




        let rsDeactivateTenantWorkflow = result {
            
            let! rsTenant = result {
                
                let! tenantFound =  tenantDto |> DbHelpers.fromDbDtoToTenant 


                let rs = deactivateTenantActivationStatusWorkflow tenantFound unvalidatedTenantActivationStatus

                printfn "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII"
                printfn "HERE THE ACTUAL %A" rs
                printfn "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII"
                          

                return rs 
            }

          
            return rsTenant
        }



        match rsDeactivateTenantWorkflow with  
        | Ok en ->
            match en with  
            | Ok ev ->
                let saveOfferRegistrationInvitationEvent = async {

                        let strTenantId = ev.Tenant.TenantId 
                        let prefix = "TENANT_With_ID_"
                        let connName = prefix + strTenantId 
                        let! store = EventStorePlayGround.create connName "tcp://admin:changeit@localhost:1113"


                        let fromRegInvToDto (aRegInv:RegistrationInvitation) =
                            let rs : RegistrationInvitationDtoTemp = {
                                RegistrationInvitationId = aRegInv.RegistrationInvitationId |> RegistrationInvitationId.value
                                Description = aRegInv.Description |> RegistrationInvitationDescription.value
                                TenantId = aRegInv.TenantId |> TenantId.value
                                StartingOn = aRegInv.StartingOn
                                Until = aRegInv.Until
                            } 
                            rs






                        let tenantDto : TenantDto = {
                            _id = strTenantId 
                            TenantId =  strTenantId
                            Name =  ev.Tenant.Name 
                            Description =  ev.Tenant.Description 
                            RegistrationInvitations =  ev.Tenant.RegistrationInvitations 
                            ActivationStatus =  ev.Tenant.ActivationStatus
                        }

                  

                        let tenantActivationStatusDeactivatedDto : TenantActivationStatusDeactivatedDto   = {   
                            Tenant = tenantDto 
                            ActivationStatus = ev.Tenant.ActivationStatus
                            Reason = "FIXTURE FOR NW"
                        }

                        let tenantActivationStatusDeactivatedEvent = ActivationStatusDeActivated tenantActivationStatusDeactivatedDto

                        let events = Seq.init 1 (fun _ ->  tenantActivationStatusDeactivatedEvent)
                        let tenantStreamIdPart1 = "TENANT_With_ID_=_"
                        let tenantStreamIdPart2 = tenantDto.TenantId
                        let tenatstreamId = tenantStreamIdPart1 + tenantStreamIdPart2
                        let! rsAppendToTenantStream = EventStorePlayGround.appendToStream store tenatstreamId  lastEventNumber events

                        
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

    let handleReactivateTenantActivationStatus 
                            (aReactivateTenantActivationStatusCommand:ReactivateTenantActivationStatusCommand)
                            :Result<TenantActivationStatusReactivatedEvent, ReactivateTenantActivationStatusError> = 






         //Helpers
        let fromRegInvToDto (aRegInv:RegistrationInvitation) =
            let rs : RegistrationInvitationDtoTemp = {
                RegistrationInvitationId = aRegInv.RegistrationInvitationId |> RegistrationInvitationId.value
                Description = aRegInv.Description |> RegistrationInvitationDescription.value
                TenantId = aRegInv.TenantId |> TenantId.value
                StartingOn = aRegInv.StartingOn
                Until = aRegInv.Until
            } 
            rs


        let fromDtoToDtoTemp (aRegInvDto:RegistrationInvitationDto):RegistrationInvitationDtoTemp = 
            let t : RegistrationInvitationDtoTemp = {
                RegistrationInvitationId = aRegInvDto.RegistrationInvitationId
                TenantId = aRegInvDto.TenantId
                Description = aRegInvDto.Description
                StartingOn = aRegInvDto.StartingOn
                Until = aRegInvDto.Until

            }
            t




        //Input

        let aReactivateTenantActivationStatusCommand = aReactivateTenantActivationStatusCommand.Data

 

        //IO at the edges



        let unvalidatedTenantActivationStatus:UnvalidatedTenantActivationStatusData = {
            TenantId = aReactivateTenantActivationStatusCommand.TenantId
            ActivationStatus =  aReactivateTenantActivationStatusCommand.ActivationStatus 
            Reason = aReactivateTenantActivationStatusCommand.Reason
            }

        

        let strTenantId = unvalidatedTenantActivationStatus.TenantId 
        let prefix = "TENANT_With_ID_=_"
        let connName = prefix + strTenantId 
        let store = EventStorePlayGround.create connName "tcp://admin:changeit@localhost:1113" |> Async.RunSynchronously 

        printfn "LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL"
        printfn "STREAM ID FOR TENANT TO REPLAY = %A" connName
        printfn "LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL"



        let foundTenantEventStreamList,lastEventNumber,nextEventNumber = EventStorePlayGround.readStream<TenantStreamEvent> store connName 0L 4095  |> Async.RunSynchronously



        printfn ""
        printfn ""
        printfn ""
        printfn "FoundTenantEventStreamList is foundTenantEventStreamList Satrt "
        printfn  "HERE THE ACTUAL foundTenantEventStreamList,_,_  = %A" foundTenantEventStreamList
        printfn "FoundTenantEventStreamList is foundTenantEventStreamList End "
        printfn ""
        printfn ""
        printfn ""




        let apply aTenant anEvent = 



            match anEvent with 
            | TenantStreamEvent.TenantCreated t ->
              t
            | TenantStreamEvent.ActivationStatusDeActivated t ->
                {aTenant with ActivationStatus = ActivationStatusDto.Disactivated}
            | TenantStreamEvent.ActivationStatusReActivated t ->
                {aTenant with ActivationStatus = ActivationStatusDto.Activated}

            | TenantStreamEvent.RegistrationInvitationOfferred  t ->  
                let regInDto = t.Invitation
                {aTenant with RegistrationInvitations = Array.append ([regInDto |> fromDtoToDtoTemp] |> List.toArray)   aTenant.RegistrationInvitations}
                


                


        let stateZero : TenantCreatedDto = {
            _id = "5c4353b53766624bce89cf91"
            TenantId = "5c4353b53766624bce89cf91"
            Name = "Le Quattro"
            Description =  "Restauration - Mets locaux et traditionnels"
            RegistrationInvitations = [] |> List.toArray
            ActivationStatus = ActivationStatusDto.Activated
        }






        let state = foundTenantEventStreamList 
                    |> Seq.fold apply stateZero

        let tenantDto:TenantDto = {
            _id = state._id
            TenantId = state.TenantId
            Name = state.Name
            Description  = state.Description
            RegistrationInvitations = state.RegistrationInvitations
            ActivationStatus = state.ActivationStatus
            
        }
                       
        

        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf ""
        printf ""
        printf ""
        printf ""
        printf "STATEEEEEE ================== %A" state
        printf ""
        printf ""
        printf ""
        printf ""
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"
        printf "-------------------------------------"




        let rsDeactivateTenantWorkflow = result {
            
            let! rsTenant = result {
                
                let! tenantFound =  tenantDto |> DbHelpers.fromDbDtoToTenant 


                let rs = reactivateTenantActivationStatusWorkflow tenantFound unvalidatedTenantActivationStatus

                printfn "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII"
                printfn "HERE THE ACTUAL %A" rs
                printfn "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII"
                          

                return rs 
            }

          
            return rsTenant
        }



        match rsDeactivateTenantWorkflow with  
        | Ok en ->
            match en with  
            | Ok ev ->
                let saveTenantActivationStatusReactevatedEvent = async {

                        let strTenantId = ev.Tenant.TenantId 
                        let prefix = "TENANT_With_ID_"
                        let connName = prefix + strTenantId 
                        let! store = EventStorePlayGround.create connName "tcp://admin:changeit@localhost:1113"


                        let fromRegInvToDto (aRegInv:RegistrationInvitation) =
                            let rs : RegistrationInvitationDtoTemp = {
                                RegistrationInvitationId = aRegInv.RegistrationInvitationId |> RegistrationInvitationId.value
                                Description = aRegInv.Description |> RegistrationInvitationDescription.value
                                TenantId = aRegInv.TenantId |> TenantId.value
                                StartingOn = aRegInv.StartingOn
                                Until = aRegInv.Until
                            } 
                            rs






                        let tenantDto : TenantDto = {
                            _id = strTenantId 
                            TenantId =  strTenantId
                            Name =  ev.Tenant.Name 
                            Description =  ev.Tenant.Description 
                            RegistrationInvitations =  ev.Tenant.RegistrationInvitations 
                            ActivationStatus =  ev.Tenant.ActivationStatus
                        }

                  

                        let tenantActivationStatusDeactivatedDto : TenantActivationStatusDeactivatedDto   = {   
                            Tenant = tenantDto 
                            ActivationStatus = ev.Tenant.ActivationStatus
                            Reason = "FIXTURE FOR NW"
                        }

                        let tenantActivationStatusDeactivatedEvent = ActivationStatusDeActivated tenantActivationStatusDeactivatedDto

                        let events = Seq.init 1 (fun _ ->  tenantActivationStatusDeactivatedEvent)
                        let tenantStreamIdPart1 = "TENANT_With_ID_=_"
                        let tenantStreamIdPart2 = tenantDto.TenantId
                        let tenatstreamId = tenantStreamIdPart1 + tenantStreamIdPart2
                        let! rsAppendToTenantStream = EventStorePlayGround.appendToStream store tenatstreamId  lastEventNumber events

                        
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
