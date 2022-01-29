namespace Lazarus.Models

open System.IO
open YamlDotNet.Serialization

type Config() =
    member val Token = "" with get, set
    member val DialogEndpoint = "" with get, set
    member val AuthEndpoint = "" with get, set
    member val Username = "" with get, set
    member val Password = "" with get, set
    member val TestGuilds: string[] = [||] with get, set

    static member ConfigDirectoryName = "Config"

    static member ConfigFilePath =
        Config.ConfigDirectoryName + "/config.yaml"

    static member ConfigDeserializer =
        DeserializerBuilder()
            .WithNamingConvention(NamingConventions.CamelCaseNamingConvention.Instance)
            .Build()

    static member LoadConfig() =
        if not (Directory.Exists Config.ConfigDirectoryName) then
            Directory.CreateDirectory(Config.ConfigDirectoryName)
            |> ignore
        else
            ()

        try
            if (File.Exists Config.ConfigFilePath) then
                Config.ConfigDeserializer.Deserialize<Config>(File.ReadAllText Config.ConfigFilePath)
            else
                Config()
        with
        | ex ->
            eprintfn $"{ex}"
            Config()
