open Xunit
open Xunit.Abstractions
open Swensen.Unquote


type Calculator() = 
    member __.Add x y = x + y + 1

let ``Given I have entered`` firstNumber continuation = 
    let calculator = Calculator()
    continuation (firstNumber, calculator)

let ``into the calculator`` (state,calculator) continuation = 
    continuation (state, calculator)

let ``And I have entered`` (firstNumber, calculator) secondNumber continuation = 
    continuation ((firstNumber, secondNumber), calculator)

let ``When I press add`` ((firstNumber, secondNumber), calculator:Calculator) continuation = 
    let actual = calculator.Add firstNumber secondNumber
    continuation actual

let ``Then the result should be`` actual (expected:int) continuation = 
    Assert.Equal(expected, actual)
    continuation

let ``on the screen`` = true




let ``Given`` context continuation =
    continuation context

let ``When`` context action continuation =
    continuation (action(context))

let ``Then`` result assertion continuation =
    assertion(result) && continuation(result)


   

type Test (output : ITestOutputHelper) =
    
    [<Fact>]
    member _.testAdd () = 
        test <@ ``Given I have entered`` 50 ``into the calculator`` 
         ``And I have entered`` 70 ``into the calculator`` 
         ``When I press add``
         ``Then the result should be`` 120 ``on the screen`` @>

    [<Fact>]
    member _.testAdd2 () = 
        test 
            <@ 
                let x = Calculator()
                x.Add 1 2 = 3
            @>

