namespace Lazarus.Services

open System
open System.Collections.Concurrent
open System.Text
open System.Threading.Channels
open System.Threading.Tasks
open Fleece
open Lazarus.Models.Config
open Lazarus.Models.Game.Serializables
open Microsoft.Extensions.Logging
open RabbitMQ.Client
open RabbitMQ.Client.Events

type AmqpService(config: Config) as x =
    let burstGameExchangeName = "burst_game"
    let burstMatchExchangeName = "burst_match"
    let mutable disposed = false
    let publishChannels = ConcurrentQueue<IModel>()
    let subscribeChannels = ConcurrentQueue<IModel>()
    let threadChannel = Channel.CreateUnbounded<GenericMatchData>()
    let responseChannel = Channel.CreateUnbounded<GenericMatchData>()

    do
        let factory = ConnectionFactory()
        factory.HostName <- config.Rabbit.Endpoint
        factory.DispatchConsumersAsync <- true
        factory.UserName <- config.Rabbit.Username
        factory.Password <- config.Rabbit.Password
        x.publishConnection <- factory.CreateConnection()
        x.subscribeConnection <- factory.CreateConnection()

        let loggerFactory =
            LoggerFactory.Create
                (fun builder ->
                    builder
                        .AddConsole()
                        .SetMinimumLevel(LogLevel.Debug)
                    |> ignore)

        x.logger <- loggerFactory.CreateLogger<AmqpService>()

        [ x.publishConnection, publishChannels
          x.subscribeConnection, subscribeChannels ]
        |> List.iter
            (fun (conn, queue) ->
                [ 1 .. (Environment.ProcessorCount / 2) ]
                |> List.iter
                    (fun _ ->
                        let channel = conn.CreateModel()
                        channel.ExchangeDeclare(burstGameExchangeName, ExchangeType.Topic)
                        channel.ExchangeDeclare(burstMatchExchangeName, ExchangeType.Topic)
                        queue.Enqueue(channel)))

        x.logger.LogInformation("AMQP connection successfully initiated")
        
        task {
            do! x.StartListeningForRequests ()
        } |> ignore
        
        task {
            do! x.StartPublishingResponses ()
        } |> ignore

    [<DefaultValue>]
    val mutable private publishConnection: IConnection

    [<DefaultValue>]
    val mutable private subscribeConnection: IConnection

    [<DefaultValue>]
    val mutable private logger: ILogger<AmqpService>
    
    override x.Finalize() = x.Dispose false
    
    member x.Reader with get() = threadChannel.Reader
    
    member private x.StartPublishingResponses () =
        task {
            let dequeueResult, channel = publishChannels.TryDequeue()
            if dequeueResult |> not then
                do! Task.Delay(TimeSpan.FromSeconds(2))
            else
                let! matchData = responseChannel.Reader.ReadAsync()
                let gameId = matchData.GameId
                let body = GenericJoinStatus.Matched matchData |> SystemTextJson.Operators.toJsonText |> Encoding.UTF8.GetBytes
                channel.BasicPublish(burstMatchExchangeName, $"ai.match.responses.{gameId}{config.Rabbit.Suffix}", body = body)
                publishChannels.Enqueue(channel)
            return! x.StartPublishingResponses ()
        }
    
    member private x.StartListeningForRequests () =
        task {
            let dequeueResult, channel = subscribeChannels.TryDequeue()
            if dequeueResult |> not then
                do! Task.Delay(TimeSpan.FromSeconds(2))
                return! x.StartListeningForRequests ()
            else
                let queue = channel.QueueDeclare().QueueName
                channel.QueueBind(queue, burstMatchExchangeName, $"ai.match.requests.*{config.Rabbit.Suffix}")
                let consumer = AsyncEventingBasicConsumer(channel)
                consumer.add_Received
                    (fun _ ea ->
                        task {
                            let _gameId = if String.IsNullOrWhiteSpace(config.Rabbit.Suffix) |> not then
                                            ea.RoutingKey[18..].Replace(config.Rabbit.Suffix, String.Empty)
                                          else
                                            ea.RoutingKey[18..]
                            let body = ea.Body.ToArray() |> Encoding.UTF8.GetString
                            let matchData = SystemTextJson.Operators.ofJsonText<GenericJoinStatus> body
                            match matchData with
                            | Ok(Matched genericMatchData) ->
                                let newPlayerIds = config.LazarusId :: genericMatchData.PlayerIds
                                let newMatchData = { genericMatchData with PlayerIds = newPlayerIds }
                                do! threadChannel.Writer.WriteAsync(newMatchData)
                                do! responseChannel.Writer.WriteAsync(newMatchData)
                            | Error errorValue ->
                                x.logger.LogError("Failed to write match data to thread channel: {Error}", errorValue)
                            ()
                        })
                channel.BasicConsume(consumer, queue, true, $"game.ai.matches.requests.consumer{config.Rabbit.Suffix}") |> ignore
                x.logger.LogInformation("Successfully started listening for incoming requests")
                subscribeChannels.Enqueue(channel)
        }

    member private x.Dispose disposing =
        if disposed then
            ()
        else if disposing then
            [ publishChannels
              subscribeChannels ]
            |> List.iter
                (fun queue ->
                    queue
                    |> Seq.iter (fun channel -> channel.Dispose())

                    queue.Clear())

            x.publishConnection.Dispose()
            x.subscribeConnection.Dispose()

            disposed <- true
        else
            ()

    interface IDisposable with
        member x.Dispose() = x.Dispose true
