#r "nuget: FSharp.Data"
#r "nuget: System.Xml.Linq"

open FSharp.Data
open System.Xml

[<Literal>]
let handoffPath = __SOURCE_DIRECTORY__ + "\salesHandoff.xml"

type PurchaseHandoff = FSharp.Data.XmlProvider<handoffPath>

let sample = PurchaseHandoff.GetSample()

let y = sample.
