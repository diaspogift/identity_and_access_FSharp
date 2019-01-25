namespace IdentityAndAcccess.EventStorePlayGround.Implementation


open IdentityAndAcccess

open IdentityAndAcccess.SerializationPlayGroung
open System
open EventStore.ClientAPI
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAccess.DatabaseTypes
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces

open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainTypes.Functions.Dto
open IdentityAndAcccess.DomainTypes.Tenant





type TenantStreamEvent =
    | TenantCreated of Dto.TenantCreated
    | ActivationStatusReActivated of Dto.ActivationStatusAndReason
    | ActivationStatusDeActivated of Dto.ActivationStatusAndReason
    | InvitationOfferred of Dto.OfferredRegistrationInvitation
    | InvitationWithdrawn of Dto.WithnrawnRegistrationInvitation



type UserStreamEvent =
    | UserRegistered of Dto.User
    | PasswordChanged of Password
    | PersonalNameChanged of Dto.FullName


type RoleStreamEvent =
    | RoleCreated of Dto.Role  
    | RoleDeleted of Dto.Role
    | RoleRenamed of Dto.Role



type GroupStreamEvent =
    | GroupCreated of Dto.GroupCreated
    | UserAddedToGroup of Dto.GroupMember
    | GroupAddedToGroup of Dto.GroupMember


















module EventStorePlayGround = 





    open EventStore.ClientAPI
    open System













    ///helpers 
    /// 
    /// 
    
    


    let unwrapGroup (aGroup:Dto.Group) : Dto.StandardGroup = 

        let unwrappedGroup = match aGroup with  
                                     | Dto.Group.Standard s -> s
                                     | Dto.Group.Internal i -> i


        unwrappedGroup


    let unwrapGroupCreated (aGroup:Dto.GroupCreated) : Dto.StandardGroup = 

        let unwrappedGroup = match aGroup with  
                                     | Dto.GroupCreated.Standard s -> s
                                     | Dto.GroupCreated.Internal i -> i


        unwrappedGroup.Group




















    

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










    let concatStreamId (p1:string) (p2:string) = p1.Trim() + p2.Trim() 
    let concatTenantStreamId = concatStreamId "TENANT_With_ID_=_"
    let concatRoleStreamId = concatStreamId "ROLE_With_ID_=_"
    let concatGroupStreamId = concatStreamId "GROUP_With_ID_=_"
    let concatUserStreamId = concatStreamId "USER_With_ID_=_"











    let applyTenantEvent (aTenant: IdentityAndAcccess.DomainTypes.Functions.Dto.Tenant) anEvent : (IdentityAndAcccess.DomainTypes.Functions.Dto.Tenant)= 

        match anEvent with 
        | TenantStreamEvent.TenantCreated t ->
            t.Tenant
        | TenantStreamEvent.ActivationStatusDeActivated statusAndreason ->
            { aTenant with ActivationStatus = statusAndreason.Status }
             
        | TenantStreamEvent.ActivationStatusReActivated statusAndreason ->
             { aTenant with ActivationStatus = statusAndreason.Status }

        | TenantStreamEvent.InvitationOfferred  offerredInvitation ->  
            { aTenant with  RegistrationInvitations = aTenant.RegistrationInvitations @ List.singleton offerredInvitation.OfferredInvitation }

        | TenantStreamEvent.InvitationWithdrawn  withdrawnInvitation ->  
            { aTenant with  RegistrationInvitations = aTenant.RegistrationInvitations @ List.singleton withdrawnInvitation.WithdrawnInvitation }

            
            
            
            


    let applyUserEvent (aUser:IdentityAndAcccess.DomainTypes.Functions.Dto.User) anEvent = 

        match anEvent with 
        | UserStreamEvent.UserRegistered user ->
          user
        | UserStreamEvent.PasswordChanged password ->
            {aUser with Password = password |> Password.value }
        | UserStreamEvent.PersonalNameChanged fullName ->
            let oldPerson = aUser.Person
            let newPerson = {oldPerson with Name = fullName }
            {aUser with Person = newPerson }

       



    let applyRoleEvent (aRole:Dto.Role) anEvent = 

        match anEvent with 
        | RoleStreamEvent.RoleCreated role ->
          role
        | RoleStreamEvent.RoleDeleted role ->
          role
        | RoleStreamEvent.RoleRenamed role ->
          role      









(* 
    let applyGroupEvent (aGroup:Dto.StandardGroup) anEvent = 

        match anEvent with 
        | GroupStreamEvent.GroupCreated group ->
            group
        | GroupStreamEvent.UserAddedToGroup memberAdded ->
            let newMemberList = memberAdded |> List.singleton 
            Dto.Standard { aGroup with Members = aGroup.Members @ newMemberList }
        
        | GroupStreamEvent.GroupAddedToGroup memberAdded ->
            let newMemberList = memberAdded |> List.singleton 
            Dto.Standard { aGroup with Members = aGroup.Members @ newMemberList }
        

 *)



    let applyGroupEvent (aGroup:Dto.StandardGroup) anEvent = 

        match anEvent with 
        | GroupStreamEvent.GroupCreated group ->
            match group with  
            | Dto.GroupCreated.Standard s -> s.Group
            | Dto.GroupCreated.Internal i -> i.Group
        | GroupStreamEvent.UserAddedToGroup memberAdded ->
            let newMemberList = memberAdded |> List.singleton 
            { aGroup with Members = aGroup.Members @ newMemberList }
        
        | GroupStreamEvent.GroupAddedToGroup memberAdded ->
            let newMemberList = memberAdded |> List.singleton 
            { aGroup with Members = aGroup.Members @ newMemberList }
        




    let loadTenantWithId tenantStreamId = 

        let store = create tenantStreamId "tcp://admin:changeit@localhost:1113" |> Async.RunSynchronously 
        let foundTenantEventStreamList,lastEventNumber,
            _ = readStream<TenantStreamEvent> store tenantStreamId 0L 4095  |> Async.RunSynchronously
   
        match foundTenantEventStreamList.Head with
        | TenantStreamEvent.TenantCreated tenantZeroState ->  
            let tenantZeroState:IdentityAndAcccess.DomainTypes.Functions.Dto.Tenant = tenantZeroState.Tenant
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

        match foundUserEventStreamList.Head with
        | UserStreamEvent.UserRegistered userZeroState ->
            let userDto =  foundUserEventStreamList |>  List.toArray |> Seq.fold applyUserEvent userZeroState
            Ok (userStreamId, userDto, lastEventNumber)
        | _ ->
            Error "Could not built tenant initial state"



    let loadGroupWithId groupStreamId = 

        printfn "=========================="
        printfn "groupStreamId not found %A" groupStreamId
        printfn "=========================="


        let store = create groupStreamId "tcp://admin:changeit@localhost:1113" |> Async.RunSynchronously 
        let foundGroupEventStreamList,lastEventNumber,
            _ = readStream<GroupStreamEvent> store groupStreamId 0L 4095  |> Async.RunSynchronously
       
        match foundGroupEventStreamList.Head with  
        | GroupStreamEvent.GroupCreated groupZeroState ->  
            let groupZeroState = groupZeroState |> unwrapGroupCreated
            let groupDto =  foundGroupEventStreamList 
                            |>  List.toArray 
                            |> Seq.fold applyGroupEvent groupZeroState
            Ok (groupStreamId, groupDto, lastEventNumber)
        | _ ->      
            Error "Could not built group initial state"

        
    let loadGroupWithGroupId (aGroupId:CommonDomainTypes.GroupId) : Result<Group.Group, string>  = 

     
        let groupStreamId = aGroupId |> GroupId.value |> concatGroupStreamId

        printfn "=========================="
        printfn "groupStreamId %A" groupStreamId
        printfn "=========================="


        let store = create groupStreamId "tcp://admin:changeit@localhost:1113" |> Async.RunSynchronously 
        let foundGroupEventStreamList,_,_ = readStream<GroupStreamEvent> store groupStreamId 0L 4095  |> Async.RunSynchronously
       
        match foundGroupEventStreamList.Head with  
        | GroupStreamEvent.GroupCreated groupZeroState -> 
            let groupZeroState = groupZeroState |> unwrapGroupCreated
            let groupDto =  foundGroupEventStreamList 
                            |>  List.toArray 
                            |> Seq.fold applyGroupEvent groupZeroState
            groupDto 
            |> Dto.Group.Standard
            |> Group.toDomain
        | _ ->      
            Error "Could not built group initial state"



    let loadGroupWithGroupMemberId : LoadGroupMemberById = 
        
        fun (aGroupMemberId:GroupMemberId) ->

     
        let groupStreamId = aGroupMemberId |> GroupMemberId.value |> concatGroupStreamId

        printfn "=========== IN loadGroupWithGroupMemberId ==============="
        printfn "groupStreamId %A" groupStreamId
        printfn "=========== IN loadGroupWithGroupMemberId ==============="


        let store = create groupStreamId "tcp://admin:changeit@localhost:1113" |> Async.RunSynchronously 
        let foundGroupEventStreamList,_,_ = readStream<GroupStreamEvent> store groupStreamId 0L 4095  |> Async.RunSynchronously
       
        match foundGroupEventStreamList.Head with  
        | GroupStreamEvent.GroupCreated groupZeroState ->  
            let groupZeroState = groupZeroState |> unwrapGroupCreated
            let groupDto =  foundGroupEventStreamList 
                            |>  List.toArray 
                            |> Seq.fold applyGroupEvent groupZeroState
            groupDto 
            |> Dto.Group.Standard
            |> Group.toDomain
        | _ ->      
            Error "Could not built group initial state"










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