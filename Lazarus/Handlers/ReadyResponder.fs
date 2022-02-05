namespace Lazarus.Handlers

open System
open System.Threading.Tasks
open Lazarus.Extensions
open Lazarus.Utilities
open Microsoft.Extensions.Logging
open Remora.Discord.API.Abstractions.Gateway.Events
open Remora.Discord.API.Abstractions.Objects
open Remora.Discord.API.Gateway.Commands
open Remora.Discord.Gateway
open Remora.Discord.Gateway.Responders

type ReadyResponder(logger: ILogger<ReadyResponder>, gatewayClient: DiscordGatewayClient) =
    member private x.UpdatePresences ct =
        task {
            do! Task.Delay(TimeSpan.FromHours(1), ct)
            if ct.IsCancellationRequested then
                logger.LogInformation "Activity update task has been cancelled"
                return ()
            else
                let newActivity = Constants.activities.Choose()
                let newPresence = UpdatePresence(ClientStatus.Online, false, Nullable(), [newActivity])
                gatewayClient.SubmitCommand(newPresence)
                return! x.UpdatePresences ct
        }
    
    interface IResponder<IReady> with
        member x.RespondAsync(gatewayEvent, ct) =
            logger.LogInformation("Successfully connected to the gateway")

            logger.LogInformation(
                "{Name}#{Discriminator} is now online",
                gatewayEvent.User.Username,
                gatewayEvent.User.Discriminator
            )
            
            task {
                x.UpdatePresences ct |> ignore
            } |> ignore

            Task.FromResult(Remora.Results.Result.FromSuccess())