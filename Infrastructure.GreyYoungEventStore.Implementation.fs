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
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces

open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainTypes.Functions.Dto
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.Functions.Dto
open IdentityAndAcccess.DomainTypes.Group





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
    | UserAssignedToRole of Dto.UserAssignedToRoleEvent
    | UserUnAssignedToRole of Dto.UserUnAssignedFromRoleEvent
    | GroupAssignedToRole of Dto.GroupAssignedToRoleEvent
    | RoleDeleted of Dto.Role
    | RoleRenamed of Dto.Role



type GroupStreamEvent =
    | GroupCreated of Dto.GroupCreated
    | UserAddedToGroup of Dto.UserAddedToGroupEvent
    | UserRemovedFromGroup of Dto.UserRemovedFromGroupEvent





type AggregateRecord<'Data> = {
    StreamId : string 
    Data : 'Data
    LastEventNum : int64
}









module EventStorePlayGround = 





    open EventStore.ClientAPI
    open System













    ///helpers 
    /// 
    /// 
    
    

















    

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
            let conn = EventStoreConnection.Create(Uri(connectionUri), connectionName)
            do! Async.AwaitTask (conn.ConnectAsync())
            return conn 
        }

    (*let subscribe (projection: Event<'T> -> unit) (getStore: Async<IEventStoreConnection>) =
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
            return (events, slice.LastEventNumber, nextEventNumber) 
        }

    let appendToStream (store: IEventStoreConnection) streamId expectedVersion newEvents = 
        async {
            let serializedEvents = [| for event in newEvents -> serialize event |]

            do!    store.AsyncAppendToStream streamId expectedVersion serializedEvents 
                   |> Async.Ignore
        }







    let toStreamId (p1:string) (p2:string) = p1.Trim() + p2.Trim() 
    let toTenantStreamId = toStreamId "tenant_id_=_"
    let toRoleStreamId = toStreamId "role_id_=_"
    let toGroupStreamId = toStreamId "group_id_=_"
    let toUserStreamId = toStreamId "user_id_=_"









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

            let newInvitationList = 
                aTenant.RegistrationInvitations 
                |> List.filter (fun reginv -> 
                    (reginv.RegistrationInvitationId <> withdrawnInvitation.WithdrawnInvitation.RegistrationInvitationId))
            { aTenant with  RegistrationInvitations = newInvitationList }

            
            
            
            


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
        | RoleStreamEvent.UserAssignedToRole  userAssignedToRole ->
         
            let newUserThatPlayMe = aRole.UsersThatPlayMe @ [userAssignedToRole.AssignedUser]
           
            { aRole with UsersThatPlayMe = newUserThatPlayMe}

        | RoleStreamEvent.UserUnAssignedToRole  userUnAssignedToRole ->

            let newUsersThatPlayMe = 
                aRole.UsersThatPlayMe 
                |> List.filter (fun ud -> ud <> userUnAssignedToRole.AssignedUser)
           
            { aRole with UsersThatPlayMe = newUsersThatPlayMe}

        | RoleStreamEvent.GroupAssignedToRole  groupAssignedToRole ->
         
            let newGroupsThatPlayMe = aRole.GroupsThatPlayMe @ [groupAssignedToRole.AssignedGroup]
           
            { aRole with GroupsThatPlayMe = newGroupsThatPlayMe}
            
        | RoleStreamEvent.RoleDeleted role ->
            role
        | RoleStreamEvent.RoleRenamed role ->
            role      






    let applyGroupEvent (aGroup:Dto.Group) (anEvent:GroupStreamEvent) :(Dto.Group) = 

        match anEvent with 
        | GroupStreamEvent.GroupCreated group ->
            group
        | GroupStreamEvent.UserAddedToGroup userAddedToGroup ->
            let newUsersList = aGroup.UsersAddedToMe @ [userAddedToGroup.AddedUser]
            { aGroup with UsersAddedToMe = newUsersList }
        
        | GroupStreamEvent.UserRemovedFromGroup userRemovedFromGroup ->
            let newUsersList = 
                aGroup.UsersAddedToMe 
                |> List.filter (fun ud -> ud <> userRemovedFromGroup.RemovedUser)
            { aGroup with UsersAddedToMe = newUsersList }
        



    let loadTenantWithId tenantStreamId = 

        let store = create tenantStreamId "tcp://admin:changeit@localhost:1113" |> Async.RunSynchronously 
        let foundTenantEventStreamList,lastEventNumber,
            _ = readStream<TenantStreamEvent> store tenantStreamId 0L 4095  |> Async.RunSynchronously
   
        match foundTenantEventStreamList.Head with
        | TenantStreamEvent.TenantCreated tenantZeroState ->  
            let tenantZeroState:IdentityAndAcccess.DomainTypes.Functions.Dto.Tenant = tenantZeroState.Tenant
            let tenantDto =  foundTenantEventStreamList |>  List.toArray |> Seq.fold applyTenantEvent tenantZeroState
            let tenantAggregate : AggregateRecord<Dto.Tenant> = {
                StreamId = toTenantStreamId tenantDto.TenantId 
                Data = tenantDto 
                LastEventNum = lastEventNumber
            }
            Ok tenantAggregate
        | _ -> 
            Error "Could not built tenant initial state"

            

        


    let loadRoleWithId roleStreamId = 

        let store = create roleStreamId "tcp://admin:changeit@localhost:1113" |> Async.RunSynchronously 
        let foundRoleEventStreamList,lastEventNumber,
            _ = readStream<RoleStreamEvent> store roleStreamId 0L 4095  |> Async.RunSynchronously
               
        match  foundRoleEventStreamList.Head with  
        | RoleStreamEvent.RoleCreated roleZeroState ->  
            let roleDto =  foundRoleEventStreamList |>  List.toArray |> Seq.fold applyRoleEvent roleZeroState

            let roleAggregate : AggregateRecord<Dto.Role> = {
                StreamId = toRoleStreamId roleStreamId
                Data = roleDto 
                LastEventNum = lastEventNumber
            }
            Ok roleAggregate
        | _ -> 
            Error "Could not built role initial state"



    let loadUserWithId userStreamId = 

        let store = create userStreamId "tcp://admin:changeit@localhost:1113" |> Async.RunSynchronously 
        let foundUserEventStreamList, lastEventNumber,
            _ = readStream<UserStreamEvent> store userStreamId 0L 4095  |> Async.RunSynchronously

       
        match foundUserEventStreamList.Head with
        | UserStreamEvent.UserRegistered userZeroState ->
            let userDto =  foundUserEventStreamList |>  List.toArray |> Seq.fold applyUserEvent userZeroState

            let userAggregate : AggregateRecord<Dto.User> = {
                StreamId = toUserStreamId userStreamId
                Data = userDto 
                LastEventNum = lastEventNumber
            }
            Ok userAggregate

        | _ ->
            Error "Could not built tenant initial state"



    let loadGroupWithId groupStreamId = 


        

        let store = create groupStreamId "tcp://admin:changeit@localhost:1113" |> Async.RunSynchronously 
        let foundGroupEventStreamList,lastEventNumber,
            _ = readStream<GroupStreamEvent> store groupStreamId 0L 4095  |> Async.RunSynchronously


        match foundGroupEventStreamList.Head with  
        | GroupStreamEvent.GroupCreated groupZeroState ->  

            let groupDto =  foundGroupEventStreamList |>  List.toArray |> Seq.fold applyGroupEvent groupZeroState

            let groupAggregate : AggregateRecord<Dto.Group> = {
                StreamId = groupStreamId
                Data = groupDto 
                LastEventNum = lastEventNumber
            }
            Ok groupAggregate
        | _ ->      
            Error "Could not built group initial state"

        
    let loadGroupWithGroupId (aGroupId:CommonDomainTypes.GroupId)   = 

     
        let groupStreamId = aGroupId |> GroupId.value |> toGroupStreamId

        let store = create groupStreamId "tcp://admin:changeit@localhost:1113" |> Async.RunSynchronously 
        let foundGroupEventStreamList, lastEventNumber,_ = readStream<GroupStreamEvent> store groupStreamId 0L 4095  |> Async.RunSynchronously
       
        match foundGroupEventStreamList.Head with  
        | GroupStreamEvent.GroupCreated groupZeroState -> 
            
            result {
                let groupDto =  foundGroupEventStreamList |>  List.toArray |> Seq.fold applyGroupEvent groupZeroState
                
                let! domain = groupDto |>  Group.toDomain

                let groupAggregate : AggregateRecord<Group.Group> = {
                    StreamId = groupStreamId
                    Data = domain 
                    LastEventNum = lastEventNumber
                    }

                return groupAggregate
                }
        | _ ->      
            Error "Could not built group initial state"





    let loadGroupWithGroupMemberId (aGroupMemberId:CommonDomainTypes.GroupMemberId) = 
        
     
        let groupStreamId = aGroupMemberId |> GroupMemberId.value |> toGroupStreamId

        let store = create groupStreamId "tcp://admin:changeit@localhost:1113" |> Async.RunSynchronously 
        let foundGroupEventStreamList, lastEventNumber,_ = readStream<GroupStreamEvent> store groupStreamId 0L 4095  |> Async.RunSynchronously
        
        
        match foundGroupEventStreamList.Head with  
        | GroupStreamEvent.GroupCreated groupZeroState -> 
            
            result {
                let groupDto =  foundGroupEventStreamList |>  List.toArray |> Seq.fold applyGroupEvent groupZeroState
                
                let! domain = groupDto |> Group.toDomain

                let groupAggregate : AggregateRecord<Group.Group> = {
                    StreamId = groupStreamId
                    Data = domain 
                    LastEventNum = lastEventNumber
                    }

                return groupAggregate
                }
        | _ ->      
            Error "Could not built group initial state"










    let toSequence = fun x -> Seq.init 1 (fun _ ->  x)



    let rec recursivePersistEventsStream streamId (position:int64) (eventsToPersist:seq<'T> []) = 

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