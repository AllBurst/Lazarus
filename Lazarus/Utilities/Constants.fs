namespace Lazarus.Utilities

open Remora.Discord.API.Abstractions.Objects
open Remora.Discord.API.Objects

module Constants =
    let outputFileName = "output.png"
    let outputAttachmentUri = $"attachment://{outputFileName}"
    let campBuddyStar = "https://cdn.discordapp.com/emojis/593518771554091011.png"
    let fSharpLogo = "https://cdn.discordapp.com/attachments/811517007446671391/939433095101313034/pngegg.png"
    let versionNumber = "0.1.1"
    let versionDate = "2022-02-05"
    let activities =
        [| "Architecting"
           "Horoscope"
           "Astrology"
           "Designing"
           "Modeling"
           "AutoCAD"
           "Rhinoceros 3D"
           "3ds Max" |]
        |> Array.map (fun s -> Activity(s, ActivityType.Game) :> IActivity)