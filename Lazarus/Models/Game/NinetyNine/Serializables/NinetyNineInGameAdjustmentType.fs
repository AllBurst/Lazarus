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
        | x -> Fleece.Uncategorized $"Invalid adjustment type `{x}`." |> Error