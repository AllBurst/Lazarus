namespace Lazarus.Models.Game.Serializables

open Fleece

type GameType
    = BlackJack
    | NinetyNine
    | ChinesePoker
    | OldMaid
    | RedDotsPicking
    | ChaseThePig
    with
    static member Parse: string -> Result<GameType, DecodeError> = function
        | "BlackJack" -> Ok BlackJack
        | "NinetyNine" -> Ok NinetyNine
        | "ChinesePoker" -> Ok ChinesePoker
        | "OldMaid" -> Ok OldMaid
        | "RedDotsPicking" -> Ok RedDotsPicking
        | "ChaseThePig" -> Ok ChaseThePig
        | x -> Uncategorized $"Invalid game type {x}." |> Error