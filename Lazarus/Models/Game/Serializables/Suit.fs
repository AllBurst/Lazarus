namespace Lazarus.Models.Game.Serializables

open Lazarus.Interfaces

type Suit
    = Club
    | Diamond
    | Heart
    | Spade
    with
    interface IValueRealizable<int> with
        member x.GetBlackJackValue() =
            match x with
            | Club -> 100
            | Diamond -> 200
            | Heart -> 300
            | Spade -> 400
        
        member x.GetChinesePokerValue() = 0
        
    static member Parse: string -> Result<Suit, Fleece.DecodeError> = function
        | "Club" -> Ok Club
        | "Diamond" -> Ok Diamond
        | "Heart" -> Ok Heart
        | "Spade" -> Ok Spade
        | x -> invalidArg x "Invalid suit."