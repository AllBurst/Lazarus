namespace Lazarus.Interfaces

open Lazarus.Models.Game.Serializables

type IGenericDealData =
    abstract member ClientType: ClientType with get, set
    abstract member GameType: GameType with get, set
    abstract member GameId: string with get, set
    abstract member PlayerId: uint64 with get, set
    abstract member ChannelId: uint64 with get, set
    abstract member PlayerName: string with get, set
    abstract member AvatarUrl: string with get, set