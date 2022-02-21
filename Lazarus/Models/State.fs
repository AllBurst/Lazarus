namespace Lazarus.Models

open Lazarus.Models.Config
open Lazarus.Models.Dialog
open Lazarus.Services

type State = {
    Config: Config
    DialogOptions: DialogOptions
    Authentication: Authentication
    AmqpService: AmqpService
}