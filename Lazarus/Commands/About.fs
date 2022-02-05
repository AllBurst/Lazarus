namespace Lazarus.Commands

open System.Collections.Generic
open System.ComponentModel
open System.IO
open Lazarus.Extensions
open Lazarus.Utilities
open Microsoft.Extensions.Logging
open Remora.Commands.Attributes
open Remora.Commands.Groups
open Remora.Discord.API.Abstractions.Objects
open Remora.Discord.API.Abstractions.Rest
open Remora.Discord.Commands.Contexts
open Remora.Discord.Extensions.Embeds
open Remora.Results

type About
    (
        context: InteractionContext,
        userApi: IDiscordRestUserAPI,
        interactionApi: IDiscordRestInteractionAPI,
        logger: ILogger<About>
    ) =
    inherit CommandGroup()

    let aboutTextPath = "Assets/about.txt"
    let aboutText = lazy ((File.ReadAllText aboutTextPath).Replace("{versionNumber}", Constants.versionNumber))

    [<Command("about")>]
    [<Description("Show information about Lloyd Sirius.")>]
    member x.Handle() =
        task {
            let! getBotResult = Utilities.getBotUser userApi logger

            match getBotResult with
            | Some bot ->
                let embed =
                    EmbedBuilder()
                        .WithAuthor(
                            "Lloyd Sirius from Camp Buddy: Scoutmaster's Season",
                            iconUrl = Constants.campBuddyStar
                        )
                        .WithColour(Colors.Lloyd.ToColor)
                        .WithThumbnailUrl(bot.GetAvatarUrl())
                        .WithDescription(aboutText.Value)
                        .WithFooter($"Lloyd Sirius: {Constants.versionNumber} | {Constants.versionDate}")
                        .WithImageUrl(Constants.fSharpLogo)
                        .Build()

                let e =
                    (embed.Entity :> IEmbed) :: [] :> IReadOnlyList<IEmbed>

                let! editResult =
                    interactionApi.EditOriginalInteractionResponseAsync(
                        context.ApplicationID,
                        context.Token,
                        embeds = e
                    )

                return
                    (if editResult.IsSuccess then
                         Result.FromSuccess()
                     else
                         Result.FromError editResult)
            | None -> return Result.FromSuccess()
        }
