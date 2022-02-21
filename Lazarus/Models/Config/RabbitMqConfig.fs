namespace Lazarus.Models.Config

type RabbitMqConfig() =
    member val Endpoint = "" with get, set
    member val Username = "" with get, set
    member val Password = "" with get, set
    member val Suffix = "" with get, set