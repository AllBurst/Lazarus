namespace Lazarus.Models.Game.Serializables

open Lazarus.Models.Game.Serializables
open Newtonsoft.Json.Linq
open Thoth.Json.Net

type GenericMatchData =
    {
        GameType: GameType
        GameId: string
        PlayerIds: uint64 list
        BaseBet: float32
    }

# nowarn "3391"    
module GenericMatchData =
    let encoder (x: GenericMatchData) =
        let jsonValue = Encode.object [
            "game_type", (match x.GameType with
                          | BlackJack -> "BlackJack"
                          | NinetyNine -> "NinetyNine"
                          | ChinesePoker -> "ChinesePoker"
                          | OldMaid -> "OldMaid"
                          | RedDotsPicking -> "RedDotsPicking"
                          | ChaseThePig -> "ChaseThePig") |> Encode.string
            "game_id", Encode.string x.GameId
            "player_ids", List.map (fun (id: uint64) -> JValue(id) :> JsonValue) x.PlayerIds |> Encode.list
            "base_bet", Encode.float32 x.BaseBet
        ]
        jsonValue["type"] <- "Matched"
        jsonValue
        
    let decoder: Decoder<GenericMatchData> =
        Decode.object (fun get ->
            {
                GameType = get.Required.Field "game_type" (Decode.string |> Decode.andThen (function
                    | "BlackJack" -> Decode.succeed BlackJack
                    | "NinetyNine" -> Decode.succeed NinetyNine
                    | "ChinesePoker" -> Decode.succeed ChinesePoker
                    | "OldMaid" -> Decode.succeed OldMaid
                    | "RedDotsPicking" -> Decode.succeed RedDotsPicking
                    | "ChaseThePig" -> Decode.succeed ChaseThePig
                    | invalid -> Decode.fail $"Failed to decode `{invalid}`."))
                GameId = get.Required.Field "game_id" Decode.string
                PlayerIds = get.Required.Field "player_ids" (Decode.list Decode.uint64)
                BaseBet = get.Required.Field "base_bet" Decode.float32
            })