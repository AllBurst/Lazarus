namespace Lazarus.Models.Game.Serializables

open Fleece.Operators
open Fleece.SystemTextJson
open FSharpPlus

type GameType
    = BlackJack
    | NinetyNine
    | ChinesePoker
    | OldMaid
    | RedDotsPicking
    | ChaseThePig