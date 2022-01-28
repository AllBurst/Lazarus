namespace Lazarus.Models

open System.IO
open YamlDotNet.Serialization
open YamlDotNet.Serialization.NamingConventions

type Config = {
    Token: string
    DialogEndpoint: string
    AuthEndpoint: string
    Username: string
    Password: string
    TestGuilds: string[]
}

module Configuration =
    let private ConfigDirectoryName = "Config"
    
    let private ConfigFilePath = ConfigDirectoryName + "/config.yaml"
    
    let private ConfigDeserializer = DeserializerBuilder()
                                         .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                         .Build()
                                         
    let private EmptyConfig = { Config.Token = ""; Config.Password = ""; Config.Username = ""; Config.AuthEndpoint = ""; Config.DialogEndpoint = "" }
    
    let LoadConfig () =
        if not (Directory.Exists ConfigDirectoryName) then
            Directory.CreateDirectory(ConfigDirectoryName) |> ignore
        else
            ()
        try
            if (File.Exists ConfigFilePath) then
                ConfigDeserializer.Deserialize<Config>(File.ReadAllText ConfigFilePath)
            else
                EmptyConfig
        with
        | ex ->
            eprintfn $"{ex}"
            EmptyConfig