namespace Lazarus.Models.Game.Serializables

open Lazarus.Models.Game.Serializables
open Thoth.Json.Net

type GenericJoinStatus
    = Matched of GenericMatchData
    
module GenericJoinStatus =
    let encoder (x: GenericJoinStatus) =
        match x with
        | Matched genericMatchData -> GenericMatchData.encoder genericMatchData
        
    let decoder: Decoder<GenericJoinStatus> =
        GenericMatchData.decoder |> Decode.andThen (fun matchData -> Matched matchData |> Decode.succeed)