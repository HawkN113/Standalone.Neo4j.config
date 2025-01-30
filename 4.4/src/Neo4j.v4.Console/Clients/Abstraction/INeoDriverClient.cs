using Neo4j.Driver;

namespace Neo4j.v4.Console.Clients.Abstraction;

public interface INeoDriverClient: IDisposable
{
    Task<EagerResult<IReadOnlyList<IRecord>>> ExecuteQueryAsync(string cypherQuery, string databaseName = "neo4j",
        CancellationToken token = default);

    Task VerifyConnectivityAsync();
}