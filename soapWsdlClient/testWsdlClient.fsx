#r "nuget: System.ServiceModel"
#r "nuget: FSharp.Data"
#r "nuget: FSharp.Data.TypeProviders"

open FSharp.Data
open FSharp.Data.TypeProviders

type String = System.String

type Wsdl1 = WsdlService<"http://api.microsofttranslator.com/V2/Soap.svc">

let ctxt = Wsdl1.GetBasicHttpBinding_LanguageService()