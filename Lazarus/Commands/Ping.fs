namespace Lazarus.Commands

open System
open System.Collections.Generic
open System.ComponentModel
open Flurl.Http
open Lazarus.Models
open Lazarus.Models.Dialog
open Microsoft.Extensions.Logging
open OneOf
open Remora.Commands.Attributes
open Remora.Commands.Groups
open Remora.Discord.API.Abstractions.Objects
open Remora.Discord.API.Abstractions.Rest
open Remora.Discord.Commands.Contexts
open Thoth.Json.Net

type Ping(context: InteractionContext, state: State, logger: ILogger<Ping>, interactionApi: IDiscordRestInteractionAPI) =
    inherit CommandGroup()

    [<Command("ping")>]
    [<Description("Check if Lloyd is online and how fast he responds.")>]
    member x.Handle() =
        task {
            let startTime = DateTime.Now

            let! result =
                interactionApi.EditOriginalInteractionResponseAsync(
                    context.ApplicationID,
                    context.Token,
                    "🏓 Wait a moment..."
                )

            let latency = DateTime.Now - startTime

            if result.IsSuccess |> not then
                logger.LogError(
                    "Failed to respond to slash command: {Reason}, inner: {Detail}",
                    result.Error.Message,
                    result.Inner
                )

                return Remora.Results.Result.FromError(result)
            else
                let difference = latency.Milliseconds

                let message =
                    $"Well, it took me {difference} milliseconds."

                let rng = Random()

                let background =
                    state.DialogOptions.Backgrounds[rng.Next(0, Array.length state.DialogOptions.Backgrounds)]

                let requestData =
                    { SpecialDialogRequestData.Background = background
                      Text = message
                      Pose = 0
                      Clothes = "xmas"
                      Face = "talk3"
                      IsHiddenCharacter = false }
                    |> SpecialDialogRequestData.encoder
                    |> Encode.toString 4

                let requestEndpoint = $"{state.Config.DialogEndpoint}/lloyd"
                do! state.Authentication.Login()

                let! response =
                    requestEndpoint
                        .WithHeader("Authorization", $"Bearer {state.Authentication.AuthToken}")
                        .WithHeader("Content-Type", "application/json")
                        .PostStringAsync(requestData)

                let! renderResult = response.GetStreamAsync()

                let attachment =
                    [| OneOf<FileData, IPartialAttachment>.FromT0
                           (FileData(Lazarus.Utilities.Constants.outputFileName, renderResult)) |]
                    :> IReadOnlyList<OneOf<FileData, IPartialAttachment>>

                let! finalResultMessage =
                    interactionApi.CreateFollowupMessageAsync(
                        context.ApplicationID,
                        context.Token,
                        "🏓 Pong!",
                        attachments = attachment
                    )

                return
                    (if finalResultMessage.IsSuccess |> not then
                         Remora.Results.Result.FromError(finalResultMessage)
                     else
                         Remora.Results.Result.FromSuccess())
        }
