namespace IdentityAndAcccess.EventStorePlayGround.Implementation



open IdentityAndAcccess.SerializationPlayGroung
open System
open EventStore.ClientAPI
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.DomainServices
open IdentityAndAccess.DatabaseTypes



type TenantStreamEvent =
    | TenantCreated of TenantProvisionedEventDto
    | RegistrationInvitationOfferred of RegistrationInvitationOfferredEventDto
    | ActivationStatusReActivated of Reason
    | ActivationStatusDeActivated of Reason 



type UserStreamEvent =
    | UserRegistered of UserRegisteredEventDto
    | PasswordChanged of Password
    | PersonalNameChanged of FullName


type RoleStreamEvent =
    | RoleCreated of RoleProvisionedEventDto



type GroupStreamEvent =
    | GroupCreated of GroupDto




type EventOld =
    | RedNew of int
    | BlueNew of int



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


            printfn "----------------------" 
            printfn "READ SLICE %A" slice
            printfn "----------------------" 

            let events = 
                slice.Events 
                |> Seq.choose deserialize<'a>
                |> Seq.toList

            printfn "----------------------" 
            printfn "READ EVENTS %A" events
            printfn "----------------------" 

            
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