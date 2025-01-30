namespace Neo4j.v4.Console.Configuration;

public sealed class NeoSettings
{
    public required string BaseBoltAddress { get; init; }
    public required string UserName { get; init; }
    public required string Password { get; init; }
}