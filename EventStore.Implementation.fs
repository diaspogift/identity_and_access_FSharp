namespace IdentityAndAcccess.EventStore.Implementation


open IdentityAndAcccess.Serialization
open System.Net



/// Integration with EventStore.

[<RequireQualifiedAccess>]
module EventStore =

    open System
    open EventStore.ClientAPI




    /// Creates and opens an EventStore connection.
    let conn (endPoint:string) =   
        let conn = EventStoreConnection.Create(new Uri(endPoint), "InputFromFileConsoleApp"); 
        conn


    /// Creates event store based repository.
    let makeRepository (conn:IEventStoreConnection) category (serialize:obj -> string * byte array, deserialize: Type * string * byte array -> obj) =

        //let streamId (id:Guid) = category + "-" + id.ToString("N").ToLower()
        let streamId (id:Guid) = id.ToString()

        printfn("---------------------------------------------")
        printfn "IN makeRepository and streamId is %A" streamId
        printfn("---------------------------------------------")

        let load (t,id) = async {
            let streamId = streamId id
            let idInt64:int64 = 1L
            let! eventsSlice = conn.ReadStreamEventsForwardAsync(streamId, idInt64, Int32.MaxValue, false) |> Async.AwaitTask
            return eventsSlice.Events |> Seq.map (fun e -> deserialize(t, e.Event.EventType, e.Event.Data))
        }

        let commit (id,expectedVersion) e = async {


            printfn("---------------------------------------------")
            printfn "IN makeRepository commit and id is %A" id
            printfn("---------------------------------------------")

            let streamId = streamId id
            let expV:int64 = expectedVersion
            let eventType,data = serialize e
            let metaData = [||] : byte array
            let eventData = new EventData(Guid.NewGuid(), eventType, true, data, metaData)


            printfn("---------------------------------------------")
            printfn "IN makeRepository commit and eventData is %A" eventData.EventId
            printfn("---------------------------------------------")
            
            if expectedVersion = 0L then 
                
                    printfn("---------------------------------------------")
                    printfn "Found Right Match"
                    printfn "Found streamId %A" streamId
                    printfn("---------------------------------------------")


            let! t = conn.AppendToStreamAsync("user", ExpectedVersion.Any, eventData) 
                    |> Async.AwaitIAsyncResult 
                    |> Async.Ignore


            return t
        }

        load,commit

    /// Creates a function that returns a read model from the last event of a stream.
    let makeReadModelGetter (conn:IEventStoreConnection) (deserialize:byte array -> _) =
        fun streamId -> async {
            let! eventsSlice = conn.ReadStreamEventsBackwardAsync(streamId, -1L, 1, false) |> Async.AwaitTask
            if eventsSlice.Status <> SliceReadStatus.Success then return None
            elif eventsSlice.Events.Length = 0 then return None
            else 
                let lastEvent = eventsSlice.Events.[0]
                if lastEvent.Event.EventNumber = 0L then return None
                else return Some(deserialize(lastEvent.Event.Data))    
        }