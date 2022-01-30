open System
open System.Threading
open System.Threading.Tasks
open Flurl.Http
open Lazarus.Extensions.RegisterCommands
open Lazarus.Handlers
open Lazarus.Models
open Lazarus.Models.Dialog
open Lazarus.Services
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Remora.Discord.API
open Remora.Discord.API.Abstractions.Gateway.Commands
open Remora.Discord.API.Abstractions.Objects
open Remora.Discord.API.Gateway.Commands
open Remora.Discord.API.Objects
open Remora.Discord.Commands.Extensions
open Remora.Discord.Commands.Services
open Remora.Discord.Gateway
open Remora.Discord.Gateway.Extensions
open Remora.Discord.Hosting.Extensions
open Remora.Discord.Interactivity.Extensions
open Thoth.Json.Net

let private activities =
    [| "Architecting"
       "Horoscope"
       "Astrology"
       "Designing"
       "Modeling"
       "AutoCAD"
       "Rhinoceros 3D"
       "3ds Max" |]
    |> Array.map (fun s -> Activity(s, ActivityType.Game) :> IActivity)

let private createHostBuilder args (state: State) =
    Host
        .CreateDefaultBuilder(args)
        .AddDiscordService(fun _ -> state.Config.Token)
        .ConfigureServices(fun _ services ->
            services
                .AddDiscordCommands(true)
                .AddInteractivity()
                .AddSingleton(state)
                .AddResponder<ReadyResponder>()
                .RegisterCommands()
                .Configure<DiscordGatewayClientOptions>(fun (opt: DiscordGatewayClientOptions) ->
                    let intents =
                        GatewayIntents.Guilds
                        ||| GatewayIntents.GuildMessages
                        ||| GatewayIntents.GuildMessageReactions

                    opt.Intents <- intents
                    opt.Presence <- UpdatePresence(ClientStatus.Online, false, Nullable(), activities))
            |> ignore)
        .ConfigureLogging(fun builder ->
            builder
                .AddFilter("System.Net.Http.HttpClient.*.LogicalHandler", LogLevel.Warning)
                .AddFilter("System.Net.Http.HttpClient.*.ClientHandler", LogLevel.Warning)
                .AddConsole()
            |> ignore)

let private getDialogOptions (config: Config) =
    task {
        let! rawStringData = config.DialogEndpoint.GetStringAsync()

        let dialogOptions =
            Decode.fromString DialogOptions.decoder rawStringData

        return
            match dialogOptions with
            | Ok options -> options
            | Error errorValue -> failwith errorValue
    }

let config = Config.LoadConfig()
let getDialogOptionsTask = getDialogOptions config
let authentication = Authentication(config.Username, config.Password, config.AuthEndpoint)
authentication.Login().GetAwaiter().GetResult()

let amqpService = new AmqpService(config)

let state =
    { State.Config = config
      State.DialogOptions = getDialogOptionsTask.GetAwaiter().GetResult()
      State.Authentication = authentication
      State.AmqpService = amqpService }

let shutdownTokenSource = new CancellationTokenSource()

Console.CancelKeyPress.Add
    (fun e ->
        e.Cancel <- true
        shutdownTokenSource.Cancel())

let host =
    (createHostBuilder [||] state)
        .UseConsoleLifetime()
        .Build()

let services = host.Services

let slashService =
    services.GetRequiredService<SlashService>()

let checkSlashSupport = slashService.SupportsSlashCommands()

if (checkSlashSupport.IsSuccess |> not) then
    printfn $"The registered commands of the bot don't support slash commands: {checkSlashSupport.Error.Message}"
else
    let testGuilds =
        config.TestGuilds
        |> Array.map UInt64.Parse
        |> Array.filter (fun id -> uint64 0 <> id)

    if (Array.isEmpty testGuilds |> not) then
        let snowflakes =
            testGuilds
            |> Array.map
                (fun id ->
                    let snowflake = DiscordSnowflake.New id

                    task {
                        let! updateResult =
                            slashService.UpdateSlashCommandsAsync(snowflake, ct = shutdownTokenSource.Token)

                        if (updateResult.IsSuccess |> not) then
                            eprintfn
                                $"Failed to update slash commands: {updateResult.Error.Message}, inner: {updateResult.Inner}"
                        else
                            ()
                    })

        task { Task.WhenAll(snowflakes) |> ignore }
        |> ignore
    else
        task {
            let! updateResult =
                task { return! slashService.UpdateSlashCommandsAsync(Nullable(), ct = shutdownTokenSource.Token) }

            if (updateResult.IsSuccess |> not) then
                eprintfn $"Failed to update slash commands: {updateResult.Error.Message}, inner: {updateResult.Inner}"
            else
                ()
        }
        |> ignore

let applicationTask = host.RunAsync(shutdownTokenSource.Token)
applicationTask.Wait()