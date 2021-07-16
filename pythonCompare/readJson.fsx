open System.IO
open System.Text.Json
open System.Text.Json.Serialization

let f = System.IO.File.ReadAllText (__SOURCE_DIRECTORY__ + "/test.json")

let options = new JsonSerializerOptions(PropertyNameCaseInsensitive = true)

type MyJson = { Hello: string }
let result = JsonSerializer.Deserialize<MyJson>(f, options)
printfn $"{result.Hello}"
