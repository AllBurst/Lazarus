namespace Lazarus.Handlers

open System.Threading.Tasks
open Microsoft.Extensions.Logging
open Remora.Discord.API.Abstractions.Gateway.Events
open Remora.Discord.Gateway.Responders

type ReadyResponder(logger: ILogger<ReadyResponder>) =
    interface IResponder<IReady> with
        member this.RespondAsync(gatewayEvent, ct) =
            logger.LogInformation("Successfully connected to the gateway")
            logger.LogInformation("{Name}#{Discriminator} is now online", gatewayEvent.User.Username, gatewayEvent.User.Discriminator)
            Task.FromResult(Remora.Results.Result.FromSuccess())