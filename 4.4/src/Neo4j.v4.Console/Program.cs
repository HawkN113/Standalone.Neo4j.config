using Cyanide.Cypher.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neo4j.v4.Console.Clients;
using Neo4j.v4.Console.Clients.Abstraction;
using Neo4j.v4.Console.Configuration;

using var host = Host.CreateDefaultBuilder(args)
    // -------------------------- Set up configuration for the application --------------------------
    .ConfigureAppConfiguration(context =>
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        context.AddJsonFile($"appsettings.{env}.json", false);
    })
    // -------------------------- Create the service provider (Dependency Injection) --------------------------
    .ConfigureServices((context, services)  =>
    {
        // Add Neo4j settings (bolt address, username, password). Neo4j uses basic authentication.
        services.Configure<NeoSettings>(context.Configuration.GetSection("NeoSettings"));

        // Add client base on Neo4j.Driver
        services.AddScoped<INeoDriverClient, NeoDriverClient>();
    })
    .Build();

const string dataBaseName = "testDb";

try
{
    // -------------------------- Driver initialization --------------------------
    // Get Neo4j client based on Neo4j.Driver
    using var neoClient = host.Services.GetRequiredService<INeoDriverClient>();

    // -------------------------- Connectivity --------------------------
    // Verify connectivity to the Neo4j server
    await neoClient.VerifyConnectivityAsync();
    Console.WriteLine($"Connection to database {dataBaseName} is verified.");

    // -------------------------- Create graph database instance --------------------------
    // Crete a new database. Do not use default and home database 'neo4j'
    // The query builder should generate the following administrative Cypher query:
    //
    //      CREATE DATABASE testDb IF NOT EXISTS
    //
    var createDbQuery = Factory.AdminQueryBuilder()
        .Create(q => q
            .WithDatabase(dataBaseName)
            .IfNotExists()
        ).Build();
    await neoClient.ExecuteQueryAsync(createDbQuery);
    Console.WriteLine($"Query: {createDbQuery}");
    Console.WriteLine($"Database {dataBaseName} created.");

    // -------------------------- Create entities --------------------------
    // Create some entities into database
    // The query builder should generate the following Cypher query:
    //
    //      CREATE (keanu:Person {name: 'Keanu Reeves'}), (laurence:Person {name: 'Laurence Fishburne'}), (keanu)-[:ACTED_IN]->(theMatrix), (laurence)-[:ACTED_IN]->(theMatrix)
    //
    // After that the query should be executed.
    var createPersonsQuery = Factory.QueryBuilder()
        .Create(q =>
            q.WithNode(new Entity("Person", "keanu", [new Field("name", "'Keanu Reeves'")]))
                .WithNode(new Entity("Person", "laurence", [new Field("name", "'Laurence Fishburne'")]))
                .WithRelation(new Entity("ACTED_IN", ""), RelationshipType.Direct, new Entity("keanu"),
                    new Entity("theMatrix"))
                .WithRelation(new Entity("ACTED_IN", ""), RelationshipType.Direct, new Entity("laurence"),
                    new Entity("theMatrix"))
        ).Build();
    await neoClient.ExecuteQueryAsync(createPersonsQuery, dataBaseName);
    Console.WriteLine($"Query: {createPersonsQuery}");
    Console.WriteLine($"Persons are created into database {dataBaseName}.");

    await host.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Fatal error: {ex.Message}, {ex.StackTrace}");
}