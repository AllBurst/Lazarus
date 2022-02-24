namespace Lazarus.Models.Game.NinetyNine.Serializables

open Fleece
open Lazarus.Interfaces
open Lazarus.Models.Game.NinetyNine.Serializables
open Lazarus.Models.Game.Serializables

type NinetyNineInGameRequestDealData() =
    interface IGenericDealData with
        member val AvatarUrl = "" with get, set
        member val ChannelId = uint64 0 with get, set
        member val ClientType = Discord with get, set
        member val GameId = "" with get, set
        member val GameType = BlackJack with get, set
        member val PlayerId = uint64 0 with get, set
        member val PlayerName = "" with get, set
    
    member val Variation = Taiwanese with get, set
    member val Difficulty = Normal with get, set
    member val Direction = Clockwise with get, set
    
    static member ToJson (x: NinetyNineInGameRequestDealData) =
        let genericDealData = x :> IGenericDealData
        jobj [
            "client_type" .= genericDealData.ClientType.ToString()
            "game_type" .= genericDealData.GameType.ToString()
            "game_id" .= genericDealData.GameId
            "player_id" .= genericDealData.PlayerId
            "channel_id" .= genericDealData.ChannelId
            "player_name" .= genericDealData.PlayerName
            "avatar_url" .= genericDealData.AvatarUrl
            "variation" .= x.Variation.ToString()
            "difficulty" .= x.Difficulty.ToString()
            "direction" .= x.Direction.ToString()
            "request_type" .= "Deal"
        ]