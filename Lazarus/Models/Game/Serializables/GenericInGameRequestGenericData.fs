namespace Lazarus.Models.Game.Serializables

open Fleece
open FSharpPlus
open Lazarus.Models.Game.Serializables

type GenericInGameRequestGenericData =
    {
        GameId: string
        GameType: GameType
        PlayerId: uint64
    }
    with
    static member ToJson (x: GenericInGameRequestGenericData) =
        jobj [
            "game_id" .= x.GameId
            "game_type" .= x.GameType.ToString()
            "player_id" .= x.PlayerId
        ]
        
    static member OfJson json =
        match json with
        | JObject o ->
            monad {
                let! gameId = o .@ "game_id"
                let! gameType = o .@ "game_type" >>= GameType.Parse
                let! playerId = o .@ "player_id"
                return {
                    GenericInGameRequestGenericData.GameId = gameId
                    GameType = gameType
                    PlayerId = playerId
                }
            }
        | x -> Decode.Fail.objExpected x