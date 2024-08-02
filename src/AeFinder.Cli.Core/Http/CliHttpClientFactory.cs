using IdentityModel.Client;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Threading;

namespace AeFinder.Cli.Http;

public class CliHttpClientFactory : ISingletonDependency
{
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(2);

    private readonly IHttpClientFactory _clientFactory;

    public CliHttpClientFactory(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public HttpClient CreateClient(string accessToken = null, TimeSpan? timeout = null)
    {
        var httpClient = _clientFactory.CreateClient(CliConsts.HttpClientName);
        httpClient.Timeout = timeout ?? DefaultTimeout;
        
        if (!accessToken.IsNullOrEmpty())
        {
            httpClient.SetBearerToken(accessToken);
        }

        return httpClient;
    }
}