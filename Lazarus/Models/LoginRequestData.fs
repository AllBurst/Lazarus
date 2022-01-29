namespace Lazarus.Models

open Thoth.Json.Net

type LoginRequestData = { UserName: string; Password: string }

module LoginRequestData =
    let encoder (x: LoginRequestData) =
        Encode.object [ "UserName", Encode.string x.UserName
                        "Password", Encode.string x.Password ]

    let decoder: Decoder<LoginRequestData> =
        Decode.object
            (fun get ->
                { UserName = get.Required.Field "UserName" Decode.string
                  Password = get.Required.Field "Password" Decode.string })
