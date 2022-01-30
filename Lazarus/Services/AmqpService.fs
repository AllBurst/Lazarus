namespace Lazarus.Services

open System
open System.Collections.Concurrent
open Lazarus.Models
open Microsoft.Extensions.Logging
open RabbitMQ.Client

type AmqpService(config: Config) as x =
    let burstGameExchangeName = "burst_game"
    let mutable disposed = false

    do
        let factory = ConnectionFactory()
        factory.HostName <- config.RabbitMqEndpoint
        factory.DispatchConsumersAsync <- true
        factory.UserName <- config.RabbitMqUsername
        factory.Password <- config.RabbitMqPassword
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

        [ x.publishConnection, x.publishChannels
          x.subscribeConnection, x.subscribeChannels ]
        |> List.iter
            (fun (conn, queue) ->
                [ 1 .. (Environment.ProcessorCount / 2) ]
                |> List.iter
                    (fun _ ->
                        let channel = conn.CreateModel()
                        channel.ExchangeDeclare(burstGameExchangeName, ExchangeType.Topic)
                        queue.Enqueue(channel)))

        x.logger.LogInformation("AMQP connection successfully initiated")

    [<DefaultValue>]
    val mutable private publishConnection: IConnection

    [<DefaultValue>]
    val mutable private subscribeConnection: IConnection

    [<DefaultValue>]
    val mutable private publishChannels: ConcurrentQueue<IModel>

    [<DefaultValue>]
    val mutable private subscribeChannels: ConcurrentQueue<IModel>

    [<DefaultValue>]
    val mutable private logger: ILogger<AmqpService>

    override x.Finalize() = x.Dispose false

    member private x.Dispose disposing =
        if disposed then
            ()
        else if disposing then
            [ x.publishChannels
              x.subscribeChannels ]
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
