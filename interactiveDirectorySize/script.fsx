#r "nuget: XPlot.Plotly"

open System.IO
open XPlot.Plotly

// just a compiler constant attribute [<Literal>]
[<Literal>]
let SrcDir =  @"C:\Users\gparmigiani\Desktop\"

//you can use __SOURCE_DIRECTORY__ if you want the dir of the script
printfn $"srcDir: {SrcDir}"

//read command args when invoked as script fsi X
for arg in fsi.CommandLineArgs do
    printfn $"{arg}"

[<Literal>]
let limit = 5

let result = 
    Directory.EnumerateFiles(SrcDir, "*", SearchOption.AllDirectories)
    |> Seq.map FileInfo
    |> Seq.groupBy (fun x -> x.DirectoryName)
    //|> Seq.filter (fun (k,v) -> not(k.Contains("bin") ||  k.Contains("obj")) )
    |> Seq.map (fun (k,v) -> k,  (v |> Seq.sumBy( fun z -> z.Length)) / ((1024.0 ** 2.0) |> int64))
    |> Seq.sortByDescending (fun (k,v) -> v)
    |> Seq.take limit


let (dirs, sizeInMb) = result |> Seq.toList |> List.unzip

let groupedTrace1 =
        Bar(
            x = dirs,
            y = sizeInMb,
            name= "BIG FILES"
        )

let chart =
    [ groupedTrace1 ]
    |> Chart.Plot
    
//just show it in browser
chart
|> Chart.Show

//or serve it in a F# web server!
  
#r "nuget: Suave"
open Suave
startWebServer defaultConfig (chart.GetHtml() |> Successful.OK)

