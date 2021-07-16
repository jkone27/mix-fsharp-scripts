    #r "nuget:neo4j.typeprovider"
    #r "nuget:Neo4jClient"

    [<Literal>]
    let connectionstring = @"http://localhost:7474/db/data"
    [<Literal>]
    let user = @"neo4j"
    [<Literal>]
    let pwd = @"password"

    type schema = Neo4j.TypeProvider.Schema<ConnectionString=connectionstring, User=user, Pwd=pwd>
    let db = new Neo4jClient.GraphClient(Uri(connectionstring), user, pwd)
    db.Connect()

    db.Cypher
        .Match("(p:" + schema.Person.NAME + ")")
        .Where( "p.born<>1973" )
        .Return<schema.Person.Proxy>("p")
        .Limit(Nullable<int>(10))
        .Results
        |> Seq.toList

