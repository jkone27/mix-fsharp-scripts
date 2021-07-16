#r "nuget:AutoMapper"

open AutoMapper
open System
open System.Linq.Expressions

type AutoMapper.IMappingExpression<'TSource, 'TDestination> with
    // The overloads in AutoMapper's ForMember method seem to confuse
    // F#'s type inference, forcing you to supply explicit type annotations
    // for pretty much everything to get it to compile. By simply supplying
    // a different name, 
    member this.ForMemberFs<'TMember>
            (destGetter:Expression<Func<'TDestination, 'TMember>>,
             sourceGetter:Action<IMemberConfigurationExpression<'TSource, 'TDestination, 'TMember>>) =
        this.ForMember(destGetter, sourceGetter)

[<CLIMutable>]
type First = { Id : Guid; Other: string }

[<CLIMutable>]
type Second = { Id: string }


type Mapping() as this =
    inherit Profile()
    do
        (this :> Profile)
            .CreateMap<Second,First>()
            .ForMemberFs((fun o -> o.Other), (fun z -> z.Ignore()))
    


let mapper = new Mapping()

let f = { Id = (Guid.NewGuid()); Other = "hey" }

let res : Second = mapper.Map<Second>(f)

printfn "%s" res.Id