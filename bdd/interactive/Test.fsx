#r "nuget: TickSpec"
#r "nuget: Unquote"
#load "FeatureRunner.fs"

open TickSpec
open System.Reflection
open System.IO
open Swensen.Unquote




type StockItem = { mutable Count : int }

let mutable stockItem = { Count = 0 }

let ign = fun [] -> ()

Interactive.Given "a customer buys a black jumper" <| ign

Interactive.Given "I have (.*) black jumpers left in stock" <| fun [n] -> 
    stockItem.Count <- (int) n

Interactive.When "he returns the jumper for a refund" <| fun [] -> 
    stockItem.Count <- stockItem.Count + 1

Interactive.Then "I should have (.*) black jumpers in stock" <| fun [n] ->
    let passed = (stockItem.Count = (int) n)
    printfn "%s" n
    if passed = false then 
        raise (System.Exception("error"))
    else
        ()

__SOURCE_DIRECTORY__ + """\Scenario.feature"""
|> File.ReadAllText
|> Interactive.toLines 
|> Interactive.Execute

__SOURCE_DIRECTORY__ + """\ScenarioRed.feature"""
|> File.ReadAllText
|> Interactive.toLines 
|> Interactive.Execute




