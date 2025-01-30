using Microsoft.Extensions.Options;
using Neo4j.Driver;
using Neo4j.v4.Console.Clients.Abstraction;
using Neo4j.v4.Console.Configuration;

namespace Neo4j.v4.Console.Clients;

internal sealed class NeoDriverClient(IOptions<NeoSettings> neoSettings)
    : INeoDriverClient, IAsyncDisposable
{
    // Use IDriver only once in the client
    private readonly IDriver _driver = GraphDatabase.Driver(neoSettings.Value.BaseBoltAddress,
        AuthTokens.Basic(neoSettings.Value.UserName, neoSettings.Value.Password));

    /// <summary>
    /// Execute Cypher query
    /// </summary>
    /// <param name="cypherQuery"></param>
    /// <param name="databaseName"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<EagerResult<IReadOnlyList<IRecord>>> ExecuteQueryAsync(
        string cypherQuery,
        string databaseName = "neo4j", 
        CancellationToken token = default)
    {
        // Validation parameters
        if (string.IsNullOrEmpty(cypherQuery))
            throw new InvalidOperationException("Cypher query is required");
        if (string.IsNullOrEmpty(databaseName))
            throw new InvalidOperationException("Database name is required");
        
        return await _driver.ExecutableQuery(cypherQuery)
            .WithConfig(new QueryConfig(database: databaseName))
            .ExecuteAsync(token);
    }

    /// <summary>
    /// Verify connectivity to Neo4j database
    /// </summary>
    public async Task VerifyConnectivityAsync()
    {
        await _driver.VerifyConnectivityAsync();
    }

    public void Dispose()
    {
        _driver.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _driver.DisposeAsync();
    }
}