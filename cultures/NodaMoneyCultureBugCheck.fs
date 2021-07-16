#r "nuget: NodaMoney"

open NodaMoney
open System.Globalization

let res = Money(amount= 17.526465m, code = "XDR")

res / 1.m


let cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

for d in cultures do

    let r = 
        try
            Some(new RegionInfo(d.LCID))
        with _ ->
            None

    if(r.IsSome) then
        //printfn $"{d.Name}.{r.Value.ISOCurrencySymbol}"
        if r.Value.ISOCurrencySymbol = "RUB" then
            printfn $"BINGO! {d.Name}"
        else
            ()
    else
        ()