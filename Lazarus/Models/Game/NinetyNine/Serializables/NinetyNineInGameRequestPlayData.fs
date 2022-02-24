namespace Lazarus.Models.Game.NinetyNine.Serializables

open Fleece
open FSharpPlus
open Lazarus.Models.Game.NinetyNine.Serializables
open Lazarus.Models.Game.Serializables

type NinetyNineInGameRequestPlayData =
    {
        GameId: string
        GameType: GameType
        PlayerId: uint64
        PlayCards: Card list
        Adjustments: NinetyNineInGameAdjustmentType list option
        SpecifiedPlayer: uint64 option
    }
    with
    static member ToJson (x: NinetyNineInGameRequestPlayData) =
        jobj [
            "game_id" .= x.GameId
            "game_type" .= x.GameType.ToString()
            "player_id" .= x.PlayerId
            "play_cards" .= x.PlayCards
            "adjustments" .= (x.Adjustments >>= fun a -> List.map (fun a' -> a'.ToString()) a |> Some)
            "request_type" .= "Play"
        ]