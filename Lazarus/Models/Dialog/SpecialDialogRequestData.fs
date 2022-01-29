namespace Lazarus.Models.Dialog

open Thoth.Json.Net

type SpecialDialogRequestData =
    {
        Background: string
        Text: string
        Pose: int
        Clothes: string
        Face: string
        IsHiddenCharacter: bool
    }
    
module SpecialDialogRequestData =
    let encoder (x: SpecialDialogRequestData) =
        Encode.object [
            "Background", Encode.string x.Background
            "Text", Encode.string x.Text
            "Pose", Encode.int x.Pose
            "Clothes", Encode.string x.Clothes
            "Face", Encode.string x.Face
            "IsHiddenCharacter", Encode.bool x.IsHiddenCharacter
        ]