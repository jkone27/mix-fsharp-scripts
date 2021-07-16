#r "nuget: SQLProvider"
#r "nuget: FSharp.Data"
#r "nuget: XPlot.Plotly"
#r "nuget: System.Data.SqlClient"

open FSharp.Data.Sql
open FSharp.Data
open XPlot.Plotly
open System.Linq
open System.Data.SqlClient

[<Literal>]
let enosisSqlDbConnection =
    "Data Source=(local);\
    Initial Catalog=enosis_dev_local;\
    User ID=enosis_dev_local;\
    Password=enosis_dev_local;\
    Connect Timeout=5;"

[<Literal>]
let schemaPath =  __SOURCE_DIRECTORY__ + "/cache.schema"

type EnosisDb = SqlDataProvider<DatabaseVendor = Common.DatabaseProviderTypes.MSSQLSERVER, 
    ConnectionString = enosisSqlDbConnection, 
    ContextSchemaPath = schemaPath
    >

let test = EnosisDb.GetDataContext()

let orderPerAffiliate = 
    query { 
        for x in test.Orders.Order do
        groupBy x.AffiliateCode into g
        select(g.Key, g.Count())
        take 10
    } 
    |> Seq.toList

let (affiliates,counts) = 
    orderPerAffiliate |> Seq.map fst, 
    orderPerAffiliate |> Seq.map snd
    
    
let chartData = new Scatter( x = affiliates, y = counts )

let chart1 =
    [orderPerAffiliate]
    |> Chart.Bar // Chart.Line (with scatter data chartData)
    |> Chart.WithWidth 700
    |> Chart.WithHeight 500

chart1 |> Chart.Show //display in browser



let test2 = test.Queue.Worker.Create(0,0,false,false,true,false,10,"QueueType2", 0)

test.GetUpdates()

test2.QueueType  
test.SubmitUpdates()


let myNewWorker = query { 
    for x in test.Queue.Worker do
    where (x.QueueType = "TEST NEW MANUAL")
    select x.Id
    headOrDefault
}