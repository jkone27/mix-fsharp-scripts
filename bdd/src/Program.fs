// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp
module Program

    open System
    open TickSpec
    open System.Diagnostics
    open System.Reflection
    
    type StockItem = { Count : int }
    let [<Given>] ``a customer buys a black jumper`` () = 
        ()
    let [<Given>] ``I have (.*) black jumpers left in stock`` (n:int) =
        printfn "%i" n
        { Count = n }
    let [<When>] ``he returns the jumper for a refund`` (stockItem:StockItem) =
        { stockItem with Count = stockItem.Count + 1 }
    let [<Then>] ``I should have (.*) black jumpers in stock`` (n:int) (stockItem:StockItem) =
        let passed = (stockItem.Count = n)
        if not passed then
            raise (Exception("error"))
        else
            ()


    do  let ass = Assembly.GetExecutingAssembly()
        let definitions = new StepDefinitions(ass)

        let lines = System.IO.File.ReadAllLines(__SOURCE_DIRECTORY__ + "/Scenario.feature")

        definitions.Execute(lines)