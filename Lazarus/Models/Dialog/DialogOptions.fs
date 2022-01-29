namespace Lazarus.Models.Dialog

open Thoth.Json.Net

type DialogOptions =
    { Characters: string []
      Backgrounds: string [] }

module DialogOptions =
    let decoder: Decoder<DialogOptions> =
        Decode.object
            (fun get ->
                { Characters = get.Required.Field "characters" (Decode.array Decode.string)
                  Backgrounds = get.Required.Field "backgrounds" (Decode.array Decode.string) })
