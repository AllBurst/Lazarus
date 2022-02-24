namespace Lazarus.Models.Game.NinetyNine.Serializables

type NinetyNineInGameAdjustmentType
    = Plus
    | Minus
    | One
    | Fourteen
    | Eleven
    with
    static member Parse: string -> Result<NinetyNineInGameAdjustmentType, Fleece.DecodeError> = function
        | "Plus" -> Ok Plus
        | "Minus" -> Ok Minus
        | "One" -> Ok One
        | "Fourteen" -> Ok Fourteen
        | "Eleven" -> Ok Eleven
        | x -> invalidArg x "Invalid Ninety-Nine adjustment type."