using System.Collections.Immutable;

namespace AeFinder.Cli;

public static class CliConsts
{
    public static readonly ImmutableDictionary<AeFinderNetwork, AeFinderEndpoint> AeFinderEndpoints =
        ImmutableDictionary.CreateRange(
            new[]
            {
                KeyValuePair.Create(AeFinderNetwork.MainNet, new AeFinderEndpoint
                {
                    AuthEndpoint = "https://indexer-auth.aefinder.io/",
                    ApiEndpoint = "https://indexer-api.aefinder.io/"
                }),
                KeyValuePair.Create(AeFinderNetwork.TestNet, new AeFinderEndpoint
                {
                    AuthEndpoint = "https://gcptest-indexer-auth.aefinder.io/",
                    ApiEndpoint = "https://gcptest-indexer-api.aefinder.io/"
                })
            });

    public const string HttpClientName = "AeFinderHttpClient";
}