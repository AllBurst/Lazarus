namespace Lazarus.Models

open Lazarus.Models
open Lazarus.Models.Dialog
open Lazarus.Services

type State = {
    Config: Config
    DialogOptions: DialogOptions
    Authentication: Authentication
}