#r "nuget: Suave"
#r "nuget: XPlot.GoogleCharts"

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful


let app =
  choose
    [ GET >=> choose
        [ path "/hello" >=> OK sample.Hello
          path "/goodbye" >=> OK "Good bye GET" ]
      POST >=> choose
        [ path "/hello" >=> OK "Hello POST"
          path "/goodbye" >=> OK "Good bye POST" ] ]

startWebServer defaultConfig app
//(Successful.OK "Hello World!")


//#r "nuget: XPlot.Plotly"
#r "nuget: XPlot.GoogleCharts"
open XPlot.GoogleCharts
[ 1 .. 10 ] |> Chart.Line |> Chart.Show



#r "nuget: Suave"
open Suave
startWebServer defaultConfig (Successful.OK "Hello World!")

open System.IO

Directory.SetCurrentDirectory(__SOURCE_DIRECTORY__)

printfn "Line: %s" __LINE__
printfn "Source Directory: %s" __SOURCE_DIRECTORY__
printfn "Source File: %s" __SOURCE_FILE__

printSourceLocation()

