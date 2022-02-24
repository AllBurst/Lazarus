namespace Lazarus.Models.Game.NinetyNine.Serializables

open Lazarus.Models.Game.NinetyNine.Serializables
open Lazarus.Models.Game.Serializables

type NinetyNineInGameRequest
    = Deal of NinetyNineInGameRequestDealData
    | Play of NinetyNineInGameRequestPlayData
    | Burst of GenericInGameRequestGenericData
    | Close of GenericInGameRequestGenericData
    with
    static member ToJson (x: NinetyNineInGameRequest) =
        match x with
        | Deal dealData -> NinetyNineInGameRequestDealData.ToJson dealData
        | Play playData -> NinetyNineInGameRequestPlayData.ToJson playData
        | Burst genericData -> GenericInGameRequestGenericData.ToJson genericData
        | Close genericData -> GenericInGameRequestGenericData.ToJson genericData