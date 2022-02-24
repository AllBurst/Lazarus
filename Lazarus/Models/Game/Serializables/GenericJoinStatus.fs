namespace Lazarus.Models.Game.Serializables

open Fleece
open FSharpPlus
open Lazarus.Models.Game.Serializables
open Thoth.Json.Net

type GenericJoinStatus
    = Matched of GenericMatchData
    with
    static member ToJson (x: GenericJoinStatus) =
        match x with
        | Matched genericMatchData -> SystemTextJson.Operators.toJson genericMatchData
        
    static member OfJson json =
        match json with
        | JObject o ->
            monad {
                let! payloadType = o .@ "type"
                match payloadType with
                | "Matched" ->
                    let! matchData = GenericMatchData.OfJson json
                    return matchData |> Matched
                | x ->
                    failwith $"Invalid payload type: {x}"
                    return GenericMatchData.Default |> Matched
            }
        | x -> Fleece.Decode.Fail.objExpected x
    
module GenericJoinStatus =
    let encoder (x: GenericJoinStatus) =
        match x with
        | Matched genericMatchData -> GenericMatchData.encoder genericMatchData
        
    let decoder: Decoder<GenericJoinStatus> =
        GenericMatchData.decoder |> Decode.andThen (fun matchData -> Matched matchData |> Decode.succeed)