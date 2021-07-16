#r "nuget: FSharp.Data"
#r "nuget: Newtonsoft.Json"
//#r "nuget: FSharpPlus"
#r "nuget: Fleece.FSharpData"

open System
open System.Reflection
open FSharp.Data
//open FSharp.Data.JsonExtensions
open Newtonsoft.Json.Linq
open Microsoft.FSharp.Quotations
//open FSharpPlus.Lens
open System.Linq.Expressions
// open Microsoft.FSharp.Quotations
// open Microsoft.FSharp.Quotations.Patterns
// open Microsoft.FSharp.Quotations.DerivedPatterns
open FSharp.Linq.RuntimeHelpers
open System.Text.RegularExpressions
open System.Collections.Generic
open FSharp.Data.Runtime.BaseTypes
open Newtonsoft.Json
open System.Runtime.CompilerServices


type JToken with
    member this.JsonValue() =
        this.ToString() |> JsonValue.Parse
    
type JToken with 
    member this.With (mutatorFunc: JToken -> 'a) =
        this |> fun y -> mutatorFunc(y) |> ignore; y

type JsonValue with 
    member this.JToken() =
        this.ToString()
        |> JToken.Parse

type JsonValue with
    member this.ToJsonDocument() =
        JsonDocument.Create(this,"")

type JsonValue with 
    member this.With (mutatorFunc: JToken -> 'a) =
        this.JToken().With(mutatorFunc).JsonValue()

let getLR expr =
    let rec getLeftRight (expr : Expression) r =
        match expr with
        | :? MethodCallExpression as mc when mc.Arguments.Count > 0 -> 
            getLeftRight (mc.Arguments.[0]) r
        | :? LambdaExpression as l -> 
            getLeftRight l.Body r
        | :? BinaryExpression as be -> 
            getLeftRight be.Right [be.Left; be.Right]
        |_ -> r
    getLeftRight expr []


let inline UpdateLeaf<'a when 'a :> IJsonDocument> (updateAction: Expr<('a -> bool)>) (jsonValue: JsonValue) =
  
      let expression = 
          updateAction 
          |> LeafExpressionConverter.QuotationToExpression
  
      let binomialResult =
          expression
          |> getLR

      // todo if not primitive, turn to JToken
      let jtoken = 
          match binomialResult with
          [l;r] ->
            let t = r.Type.Name.ToLower()
            match t with   
            | "jsonvalue" -> 
                let lambda = r.Reduce()
                let r = Expression.Lambda(lambda).Compile().DynamicInvoke()
                (r :?> JsonValue).JToken()
            | "ijsondocument" -> 
                let lambda = r.Reduce()
                let r = Expression.Lambda(lambda).Compile().DynamicInvoke()
                (r :?> IJsonDocument).JsonValue.JToken()
            | "ijsondocument[]" -> 
                let lambda = r.Reduce()
                let r = Expression.Lambda(lambda).Compile().DynamicInvoke()
                let stringList =
                    (r :?> IJsonDocument[]) 
                    |> Array.map (fun x -> x.JsonValue.JToken())
                
                let jarrayString = String.Join(",", stringList)
                
                $"[{jarrayString}]"
                |> JArray.Parse
                :> JToken
            |_ ->  
                let tOption = typeof<option<_>>.GetGenericTypeDefinition()
                let lambda = r.Reduce()
                let r = Expression.Lambda(lambda).Compile().DynamicInvoke()
                match r with
                | null -> JValue.CreateNull() :> JToken
                |_ as o when r.GetType().IsGenericType && r.GetType().GetGenericTypeDefinition() = tOption ->
                    try
                        //Some(|....|)
                        let strOption = Newtonsoft.Json.JsonConvert.SerializeObject(o)
                        let opt = Newtonsoft.Json.JsonConvert.DeserializeObject<Option<Object>>(strOption)
                        JValue(opt.Value) :> JToken
                    with _ ->
                         JValue.CreateNull() :> JToken
                |_ -> JValue(r) :> JToken
                
                 
      let left = 
          match binomialResult with
          |[l;r] -> l
          |_ -> expression

      let cleanedStr = Regex.Replace(left.ToString(), @"\t|\n|\r", "")

      let invertedCalls = 
          Regex.Matches(cleanedStr, "\,\s+(?<Prop>(\")?(\w|\$)+(?<IsDigit>\")?)\)")
          |> Seq.map (fun m -> (m.Groups.["Prop"].Value.Replace("\"",""), String.IsNullOrEmpty(m.Groups.["IsDigit"].Value)))
          |> Seq.fold (fun acc next -> 
              let prop,isDigit = next
              match isDigit with
              |true ->
                  let q = acc |> Seq.ofList |> Queue
                  let p = q.Dequeue()
                  $"{p}.[{prop}]" :: (q |> List.ofSeq)
              |false -> 
                  prop :: acc
          ) []


      let key = invertedCalls.[0]
      let jsonPath = System.String.Join(".", invertedCalls |> Seq.skip 1 |> Seq.rev)
  
      jsonValue.With(fun x -> x.SelectToken(jsonPath).[key] <- jtoken)


let inline Change<'a when 'a :> IJsonDocument>(updateAction: Expr<('a -> bool)>) (jsonDocument : 'a) =
    UpdateLeaf updateAction jsonDocument.JsonValue
    |> fun x -> JsonDocument.Create(x,"") :?> 'a

[<Extension>]
type ExtensionMethod() =
    [<Extension>]
    static member inline Change<'a when 'a :> IJsonDocument>(this: 'a, updateAction: Expr<('a -> bool)>) =
        this.JsonValue
        |> UpdateLeaf updateAction 
        |> fun x -> JsonDocument.Create(x,"") :?> 'a

[<Literal>]
let json =
    """
    { "object" : {
        "nested" : 1,
        "unknown" : null,
        "second" : {
            "name" : "john",
            "third" : [
                {
                    "fouth":"hi"
                }
            ]
        },
        "date" : "2012-04-23T18:25:43.511Z",
        "guid" : "ad8d6b87-833f-4b6e-98bc-52b1cfbc814e"
    }}
    """

type NestedProvider = JsonProvider<json>

let t = NestedProvider.GetSample()

t.Change(<@ fun x -> x.Object.Second.Name = "hi"@>)

t.Change(<@ fun x -> 
    x.Object.Second = NestedProvider.Second("wacko", 
        [|  NestedProvider.Third("he") ; NestedProvider.Third("ho") |])@>)


let testMe x =
   $"HELLO {x + 5}"

t.Change(<@ fun x -> x.Object.Second.Third = [|  NestedProvider.Third($"{testMe 4}") |] @>)
  

t.Change(<@ fun x ->  x.Object.Date = DateTimeOffset.UtcNow @>)    


t.Change(<@ fun x -> x.Object.Guid = Guid.NewGuid() @>)   


t.Change(<@ fun x -> x.Object.Unknown.JsonValue = JsonValue.String($"{Guid.NewGuid()}") @>)


NestedProvider.GetSample()
|> Change <@ fun x ->  x.Object.Date = DateTimeOffset.UtcNow @>
|> Change <@ fun x ->  x.Object.Guid = Guid.NewGuid() @>
|> Change <@ fun x ->  x.Object.Unknown.JsonValue = JsonValue.String($"{Guid.NewGuid()}") @>
|> Change <@ fun x ->  x.Object.Second.Third = [|  NestedProvider.Third($"{testMe 4}") |] @>


//////
/// 

// type Json =
//     | JObj of Json seq
//     | JProp of string * Json
//     | JArr of Json seq
//     | JVal of obj

// let (!!) (o: obj) = JVal o

// let rec toJson2 = function
//     | JVal v -> new JValue(v) :> JToken
//     | JProp(name, (JProp(_) as v)) -> new JProperty(name, new JObject(toJson2 v)) :> JToken
//     | JProp(name, v) -> new JProperty(name, toJson2 v) :> JToken
//     | JArr items -> new JArray(items |> Seq.map toJson2) :> JToken
//     | JObj props -> new JObject(props |> Seq.map toJson2) :> JToken

// let rec toJson = function
//     | JsonValue.String v -> JVal v
//     | JsonValue.Number v -> JVal v
//     | JsonValue.Float v -> JVal v
//     | JsonValue.Boolean v -> JVal v
//     | JsonValue.Null -> JVal "null"
//     | JsonValue.Array items -> JArr(items |> Seq.map toJson)
//     | JsonValue.Record props -> JObj(props |>  Seq.map (fun (k,v) -> JProp(k,(toJson v))))


//nice
JsonConvert.SerializeObject(None)
JsonConvert.DeserializeObject<Option<Object>>("null")

//also nice
JsonConvert.SerializeObject(null)
JsonConvert.DeserializeObject<Option<Object>>("null")