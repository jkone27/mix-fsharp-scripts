// Learn more about F# at http://fsharp.org

open System

//a simple class with one function and one property
type MyClass() =

    static let staticField = "hello"

    //static constructor workaround
    static do printfn "%A" staticField

    let mutable privateProperty = "hey"
    let privateSum a b = a + b

    member this.SomeFunction () = 3
    member this.MyMutableProperty
        with get() = "test"
        and private set(value) = privateProperty <- value

    member private this.MyProperty = "hello" //private get
    member val MyAutoProperty = "same" //get
    member val MyMutableAutoProperty = "test" 
        with get,set 

    //another constructor
    new (x: string, y: string) as this =
        this.MyMutableProperty <- x + y
        MyClass()

//base class (parent)       
type BaseClass(param1) =
   member this.Param1 = param1
   abstract member Pi : float 
   default this.Pi = 3.14 //abstract default (no virtual! nice)

//subclass (child)
type DerivedClass(param1, param2) =
   inherit BaseClass(param1)
   member this.Param2 = param2
   member this.Pi = 3.143562753 //override abstract

[<AbstractClass>] //abstract class
type Animal() =
   abstract member MakeNoise: unit -> unit 

type Dog() =
    inherit Animal()
    override this.MakeNoise () = printfn "bau" //abstract override

//interfaces have no constructor, being purely abstract they dont need decorations
type IMyPerson =
    abstract member FirstName : string
    abstract member LastName : string
    abstract member DoSomething : unit -> unit

type APinoPerson() =
    interface IMyPerson with 
        member val FirstName = "Pino"
        member val LastName = "Ole"
        member this.DoSomething () = printfn "ciao!"
let pino = new APinoPerson() :> IMyPerson
pino.DoSomething() //interface members need cast to be visible for access (nice!)

//object expression (similar to record pattern matching, a sort of object pattern matching)
let implicitImplementation = { new IMyPerson with 
        member this.FirstName = "test"
        member this.LastName = "ole"
        member this.DoSomething () = ()
    }


let x = MyClass()
x.MyMutableAutoProperty <- "F#"
printfn "Hello World from %A" x.MyMutableAutoProperty
   
