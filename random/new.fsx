#r "nuget: FSharp.Data"
open FSharp.Data

///// JSON

[<Literal>]
let jsonFilePath = __SOURCE_DIRECTORY__ + @"\testjson.json"

type MyJson = FSharp.Data.JsonProvider<jsonFilePath>

let jsonSample = MyJson.Load(__SOURCE_DIRECTORY__ + "/prod.json")

jsonSample.

///// CSV

[<Literal>]
let csvFilePath = __SOURCE_DIRECTORY__ + @"\testcsv.csv"

type MyCsv = FSharp.Data.CsvProvider<csvFilePath>

let myCsv = MyCsv.Load(__SOURCE_DIRECTORY__ + "/prod.csv")

for r in myCsv.Rows do
    printfn "%A" (r.AGE, r.SURNAME)

/////// XML

[<Literal>]
let xmlFilePath = __SOURCE_DIRECTORY__ + @"\books.xml"

type Books = FSharp.Data.XmlProvider<xmlFilePath>

let booksSample = Books.GetSample()

let firstBook : Books.Book = booksSample.Books.[0]

firstBook.Author


