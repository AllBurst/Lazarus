namespace Lazarus.Models.Authentication

open System
open Thoth.Json.Net

type LoginResponseData =
    {
        Token: string
        Expiry: DateTime
    }
    
module LoginResponseData =
    let decoder: Decoder<LoginResponseData> =
        Decode.object (fun get ->
            {
                Token = get.Required.Field "token" Decode.string
                Expiry = get.Required.Field "expiry" Decode.string |> DateTime.Parse
            })