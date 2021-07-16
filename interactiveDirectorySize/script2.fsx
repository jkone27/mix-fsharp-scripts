open System.IO

[<Literal>]
let SrcDir = @"C:/Octopus"

printfn "srcDir: %A" SrcDir

[<Literal>]
let limit = 5

try

    let result = 
        Directory.EnumerateFiles(SrcDir, "*", SearchOption.AllDirectories)
        |> Seq.map FileInfo
        |> Seq.groupBy (fun x -> x.DirectoryName)
        |> Seq.filter (fun (k,v) -> not(k.Contains("bin") ||  k.Contains("obj")) )
        |> Seq.map (fun (k,v) -> k,  (v |> Seq.sumBy( fun z -> z.Length)) / (1024.0 ** 2.0 |> int64))
        |> Seq.sortByDescending (fun (k,v) -> v)
        |> Seq.map (fun (dir, sizeInMb) -> (sprintf "%A ~ %A MB" dir sizeInMb))
        |> Seq.take limit

    printfn "biggest top %A folders" limit


    for r in result do
        printfn "result %A" r
with ex ->
    printfn "ERROR: %s" (ex.Message)

0