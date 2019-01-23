namespace IdentityAndAcccess.EventStorePlayGround.Implementation



open IdentityAndAcccess.SerializationPlayGroung
open System
open EventStore.ClientAPI
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAccess.DatabaseTypes
open IdentityAndAcccess.DomainTypes.Tenant



type TenantStreamEvent =
    | TenantCreated of TenantDto
    | ActivationStatusReActivated of TenantActivationStatusReactivatedDto
    | ActivationStatusDeActivated of TenantActivationStatusDeactivatedDto 
    | InvitationOfferred of RegistrationInvitationOfferredDto 
    | InvitationWithdrawn of RegistrationInvitationWithdrawnDto 



type UserStreamEvent =
    | UserRegistered of UserDto
    | PasswordChanged of Password
    | PersonalNameChanged of FullName


type RoleStreamEvent =
    | RoleCreated of RoleDto    
    | RoleDeleted of RoleDto
    | RoleRenamed of RoleDto



type GroupStreamEvent =
    | GroupCreated of GroupDto
    | UserAddedToGroup of UserAddedToGroupDto


















module EventStorePlayGround = 





    open EventStore.ClientAPI
    open System







    

    type IEventStoreConnection with
        member this.AsyncConnect() = this.ConnectAsync() |> Async.AwaitTask
        member this.AsyncReadStreamEventsForward stream start count resolveLinkTos =
            this.ReadStreamEventsForwardAsync(stream, start, count, resolveLinkTos)
            |> Async.AwaitTask
        member this.AsyncAppendToStream stream expectedVersion events =
            this.AppendToStreamAsync(stream, expectedVersion, events)
            |> Async.AwaitTask


    let deserialize<'a> (event: ResolvedEvent) = deserializeUnion<'a>  event.Event.EventType event.Event.Data
    let deserialize'<'b> (event: ResolvedEvent)  = deserializeUnion<'b>   event.Event.EventType event.Event.Data

    let serialize event = 
        let typeName, data = serializeUnion event
        EventData(Guid.NewGuid(), typeName, true, data, null)

    let create connectionName connectionUri = 
        async {
            let conn = EventStoreConnection.Create(new Uri(connectionUri), connectionName)
            do! Async.AwaitTask (conn.ConnectAsync())
            return conn 
        }

(*     let subscribe (projection: Event<'T> -> unit) (getStore: Async<IEventStoreConnection>) =
        async {
        let! store = getStore
        let credential = SystemData.UserCredentials("admin", "changeit")
        do! 
            Async.AwaitTask <| store.SubscribeToAllAsync(true, (fun _ e -> deserialize e "" |> Option.iter projection), userCredentials = credential) |> Async.Ignore
        return store }
        |> Async.RunSynchronously *)

    let readStream<'a> (store: IEventStoreConnection) streamId version count = 
        
        async {


            let! slice = store.AsyncReadStreamEventsForward streamId version count true
            let events = 
                slice.Events 
                |> Seq.choose deserialize<'a>
                |> Seq.toList

            let nextEventNumber = 
                if slice.IsEndOfStream 
                then None 
                else Some slice.NextEventNumber

            return events, slice.LastEventNumber, nextEventNumber 
        }

    let appendToStream (store: IEventStoreConnection) streamId expectedVersion newEvents = 
        async {
            let serializedEvents = [| for event in newEvents -> serialize event |]

            do!    store.AsyncAppendToStream streamId expectedVersion serializedEvents 
                   |> Async.Ignore
        }









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
                  
        | TenantStreamEvent.InvitationOfferred  t ->  
            let regInDto = t.Invitation
            {aTenant with 
                RegistrationInvitations = Array.append ([regInDto |> fromDtoToDtoTemp] |> List.toArray)   aTenant.RegistrationInvitations}     
        
        | TenantStreamEvent.InvitationWithdrawn  t ->  
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
        | RoleStreamEvent.RoleDeleted t ->
          t       
        | RoleStreamEvent.RoleRenamed t ->
          t             










    let applyGroupEvent (aGroup:GroupDto) anEvent = 

        match anEvent with 
        | GroupStreamEvent.GroupCreated g ->
          g

        | GroupStreamEvent.UserAddedToGroup g ->
          
          let groupMemberDto : GroupMemberDto = {
              MemberId = g.User.UserId
              TenantId = g.User.TenantId
              Name = g.User.Username
              Type = GroupMemberTypeDto.User
          }

          let groupMemberDtoL = groupMemberDto |> Array.singleton 
          let newMembers = groupMemberDtoL |> Array.append aGroup.Members  

          {aGroup with Members = newMembers}
        


    let loadTenantWithId tenantStreamId = 

        let store = create tenantStreamId "tcp://admin:changeit@localhost:1113" |> Async.RunSynchronously 
        let foundTenantEventStreamList,lastEventNumber,
            _ = readStream<TenantStreamEvent> store tenantStreamId 0L 4095  |> Async.RunSynchronously

        printfn "-----------------------------------------------------------------------------"
        printfn " foundTenantEventStreamList = -----------------------------------------------"
        printfn "---- ---------------------%A ---------------------" foundTenantEventStreamList
        printfn "-----------------------------------------------------------------------------"
    
       
        
        match foundTenantEventStreamList.Head with
        | TenantStreamEvent.TenantCreated tenantZeroState ->  
            let tenantDto =  foundTenantEventStreamList |>  List.toArray |> Seq.fold applyTenantEvent tenantZeroState
            Ok (tenantStreamId, tenantDto, lastEventNumber)
        | _ -> 
            Error "Could not built tenant initial state"

            

        


    let loadRoleWithId roleStreamId = 

        let store = create roleStreamId "tcp://admin:changeit@localhost:1113" |> Async.RunSynchronously 
        let foundRoleEventStreamList,lastEventNumber,
            _ = readStream<RoleStreamEvent> store roleStreamId 0L 4095  |> Async.RunSynchronously
               
        match  foundRoleEventStreamList.Head with  
        | RoleStreamEvent.RoleCreated roleZeroState ->  
            let roleDto =  foundRoleEventStreamList |>  List.toArray |> Seq.fold applyRoleEvent roleZeroState
            Ok (roleStreamId, roleDto, lastEventNumber)
        | _ -> 
            Error "Could not built tenant initial state"



    let loadUserWithId userStreamId = 

        let store = create userStreamId "tcp://admin:changeit@localhost:1113" |> Async.RunSynchronously 
        let foundUserEventStreamList, lastEventNumber,
            _ = readStream<UserStreamEvent> store userStreamId 0L 4095  |> Async.RunSynchronously

        printfn "foundUserEventStreamList = %A" foundUserEventStreamList
        printfn "userStreamId = %A" userStreamId
        printfn "foundUserEventStreamList = %A" foundUserEventStreamList
        printfn "userStreamId = %A" userStreamId
        printfn "foundUserEventStreamList = %A" foundUserEventStreamList
        printfn "userStreamId = %A" userStreamId
        printfn "foundUserEventStreamList = %A" foundUserEventStreamList
        printfn "userStreamId = %A" userStreamId

        match foundUserEventStreamList.Head with
        | UserStreamEvent.UserRegistered userZeroState ->
            let userDto =  foundUserEventStreamList |>  List.toArray |> Seq.fold applyUserEvent userZeroState
            Ok (userStreamId, userDto, lastEventNumber)
        | _ ->
            Error "Could not built tenant initial state"



    let loadGroupWithId groupStreamId = 

        let store = create groupStreamId "tcp://admin:changeit@localhost:1113" |> Async.RunSynchronously 
        let foundGroupEventStreamList,lastEventNumber,
            _ = readStream<GroupStreamEvent> store groupStreamId 0L 4095  |> Async.RunSynchronously
       
        match foundGroupEventStreamList.Head with  
        | GroupStreamEvent.GroupCreated groupZeroState ->  
            let groupDto =  foundGroupEventStreamList 
                            |>  List.toArray 
                            |> Seq.fold applyGroupEvent groupZeroState
            Ok (groupStreamId, groupDto, lastEventNumber)
        | _ ->      
            Error "Could not built group initial state"

        







    let concatStreamId (p1:string) (p2:string) = p1.Trim() + p2.Trim() 
    let concatTenantStreamId = concatStreamId "TENANT_With_ID_=_"
    let concatRoleStreamId = concatStreamId "ROLE_With_ID_=_"
    let concatGroupStreamId = concatStreamId "GROUP_With_ID_=_"
    let concatUserStreamId = concatStreamId "USER_With_ID_=_"





    let toSequence = fun x -> Seq.init 1 (fun _ ->  x)



    let rec recursivePersistEventsStream streamId (position:int64) (eventsToPersist:seq<'T> array) = 

        let store = create streamId "tcp://admin:changeit@localhost:1113" |> Async.RunSynchronously 


        let eventsToPersist = eventsToPersist |> Array.toList

        match eventsToPersist with
        | [] -> ()
        | head::tail -> 

            head
            |> appendToStream store streamId  position 
            |> Async.RunSynchronously

            let nextStreamPostion = position + 1L
            let remainingEvents = tail |> List.toArray


            recursivePersistEventsStream streamId nextStreamPostion remainingEvents