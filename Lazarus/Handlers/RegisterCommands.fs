module Lazarus.Handlers.RegisterCommands

open Microsoft.Extensions.DependencyInjection
open Remora.Commands.Extensions
open Remora.Discord.Pagination.Extensions

type IServiceCollection with
    member x.RegisterCommands() =
        x.AddCommandTree()
            .WithCommandGroup<Lazarus.Commands.Ping>()
            .WithCommandGroup<Lazarus.Commands.About>()
            |> ignore
        x.AddPagination()