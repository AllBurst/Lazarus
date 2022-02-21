namespace Lazarus.Services

open System
open Flurl.Http
open Lazarus.Models.Authentication
open Thoth.Json.Net

type Authentication(username: string, password: string, authEndpoint: string) =
    member val AuthToken = "" with get, set
    member val Expiry: DateTime option = None with get, set
    
    member x.Login () =
        task {
            if (x.Expiry.IsSome && DateTime.Now > x.Expiry.Value) || x.Expiry.IsNone then
                let loginRequestData =
                    {
                        LoginRequestData.UserName = username
                        Password = password
                    }
                    |> LoginRequestData.encoder
                    |> Encode.toString 4
                let! response = authEndpoint.PostJsonAsync(loginRequestData)
                let! rawString = response.GetStringAsync()
                let authResponse = Decode.fromString LoginResponseData.decoder rawString
                match authResponse with
                | Ok loginResponseData ->
                    x.AuthToken <- loginResponseData.Token
                    x.Expiry <- Some loginResponseData.Expiry
                | Error errorValue -> eprintfn $"An error occurred when login against server: {errorValue}"
            else
                ()
        }