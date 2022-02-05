module Lazarus.Extensions

open System
open System.Collections.Generic
open System.Collections.Immutable
open System.Drawing
open Lazarus.Utilities
open Remora.Discord.API.Abstractions.Objects

type Colors with
    member x.ToColor =
        match x with
        | Lloyd -> Color.FromArgb(124, 162, 209)
        | FSharp -> Color.FromArgb(55, 139, 186)

type IEnumerable<'T> with
    member x.Choose() =
        let random = Random()
        let arr = x.ToImmutableArray()
        arr.[random.Next(0, arr.Length)]

    member x.ChooseMultiple count =
        let random = Random()
        let elements = List(x)

        let rec loop n (arr: List<'T>) acc =
            match n with
            | 0 -> acc
            | _ ->
                let i = random.Next(0, arr.Count)
                let elem = arr.[i]
                arr.RemoveAt(i)
                loop (n - 1) arr (elem :: acc)

        loop count elements []

type IUser with
    member x.GetAvatarUrl (?size: int) =
        let actualSize =
            match size with
            | Some value -> value
            | None -> 1024

        let num =
            if actualSize >= 16 && actualSize <= 2048 then
                Math.Log(float actualSize, 2.0)
            else
                raise (ArgumentOutOfRangeException(nameof size))

        if num < 4.0 || num > 11.0 || num % 1.0 <> 0.0 then
            raise (ArgumentOutOfRangeException(nameof size))
        else
            let avatarHash = x.Avatar.Value

            let str1 =
                if String.IsNullOrWhiteSpace(avatarHash) |> not then
                    if avatarHash.StartsWith("a_") then
                        "gif"
                    else
                        "png"
                else
                    "png"

            (if String.IsNullOrWhiteSpace(avatarHash) |> not then
                 $"https://cdn.discordapp.com/avatars/{x.ID.Value}/{avatarHash}.{str1}?size={actualSize}"
             else
                 $"https://cdn.discordapp.com/embed/avatars/{x.Discriminator % (5 |> uint16)}.{str1}?size={actualSize}")

type IGuildMember with
    member x.GetAvatarUrl (?size: int) =
        match size with
        | Some value -> x.User.Value.GetAvatarUrl value
        | None -> x.User.Value.GetAvatarUrl ()