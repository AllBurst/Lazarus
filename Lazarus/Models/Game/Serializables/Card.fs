namespace Lazarus.Models.Game.Serializables

open Fleece
open FSharpPlus
open Lazarus.Interfaces
open Lazarus.Models.Game.Serializables

type Card =
    {
        Suit: Suit
        Number: int
        IsFront: bool
    }
    with
    interface IValueRealizable<int []> with
        member x.GetBlackJackValue() =
            let suit = x.Suit :> IValueRealizable<int>
            let cardValue = suit.GetBlackJackValue() + (if x.Number >= 10 then 10 else x.Number)
            let values = if x.Number = 1 then
                             [cardValue; (suit.GetBlackJackValue() + 11)]
                         else
                             [cardValue]
            Array.ofList values
        
        member x.GetChinesePokerValue() =
            if x.Number = 1 then 14 else x.Number
            
    static member CanCombine card1 card2 =
        match (card1.Number, card2.Number) with
        | 1, 9 | 9, 1 -> true
        | 2, 8 | 8, 2 -> true
        | 3, 7 | 7, 3 -> true
        | 4, 6 | 6, 4 -> true
        | 5, 5 -> true
        | 10, 10 -> true
        | 11, 11 -> true
        | 12, 12 -> true
        | 13, 13 -> true
        | _ -> false
        
    static member private Create suit number isFront =
        {
            Card.Suit = suit
            Number = number
            IsFront = isFront
        }
        
    static member ToJson (x: Card) =
        jobj [
            "suit" .= x.Suit.ToString()
            "number" .= x.Number
            "is_front" .= x.IsFront
        ]
        
    static member OfJson json =
        match json with
        | JObject o -> Card.Create <!> (o .@ "suit" >>= Suit.Parse) <*> (o .@ "number") <*> (o .@ "is_front")
        | x -> Decode.Fail.objExpected x