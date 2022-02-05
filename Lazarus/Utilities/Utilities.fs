module Lazarus.Utilities.Utilities

open Microsoft.Extensions.Logging
open Remora.Discord.API.Abstractions.Rest

let getBotUser (userApi: IDiscordRestUserAPI) (logger: ILogger) =
        task {
            let! result = userApi.GetCurrentUserAsync()
            if result.IsSuccess then
                return Some result.Entity
            else
                logger.LogError("Failed to get bot user: {Reason}, inner: {Inner}", result.Error.Message, result.Inner)
                return None
        }