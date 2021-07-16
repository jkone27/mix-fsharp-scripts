
#r "nuget: FSharp.Data"
open FSharp.Data

[<Literal>]
let csvPath = __SOURCE_DIRECTORY__ + "/test.csv"

type MyCsv = CsvProvider<csvPath, ",">

for r in MyCsv.GetSample().Rows do
    printfn $"{result.NAME}"
