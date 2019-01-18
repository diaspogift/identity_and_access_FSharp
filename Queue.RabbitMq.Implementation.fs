namespace IdentityAndAccess.RabbitMQ.FSharp

open RabbitMQ.Client
open RabbitMQ.Client.Events
open System.Text

module Client =
    
    
    type Queue = { Name: string; Read: unit -> string; Publish: string -> unit }

    let openConnection address =
        let factory = new ConnectionFactory(HostName = address)
        factory.CreateConnection()

    // I need to declare the type for connection because F# can't infer types on classes
    let openChannel (connection:IConnection) = connection.CreateModel()

    let declareQueue (channel:IModel) queueName = channel.QueueDeclare( queueName, false, false, false, null )

    let readFromQueue (consumer:QueueingBasicConsumer) queueName =
        let ea = consumer.Queue.Dequeue()
        let body = ea.Body
        let message = Encoding.UTF8.GetString(body)
        message

    let publishToQueue (channel:IModel) queueName (message:string) =
        let body = Encoding.UTF8.GetBytes(message)
        channel.BasicPublish("", queueName, null, body)

    // I don't have to declare the type of connection, because F# can infer the type from my call to openChannel
    let connectToQueue connection (channel:IModel) queueName =
        declareQueue channel queueName |> ignore

        {Name = queueName;
        Read = (fun () ->
                        let ea = channel.BasicGet(queueName, true)
                        if ea <> null then
                            let body = ea.Body
                            let message = Encoding.UTF8.GetString(body)
                            message
                        else
                            "");
        Publish = (publishToQueue channel queueName)}