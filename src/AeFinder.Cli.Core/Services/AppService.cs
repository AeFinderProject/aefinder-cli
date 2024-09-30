using System.Text;
using AeFinder.Cli.Auth;
using AeFinder.Cli.Http;
using AeFinder.Cli.Options;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http;
using Volo.Abp.Threading;

namespace AeFinder.Cli.Services;

public class AppService : IAppService, ITransientDependency
{
    private readonly IAuthService _authService;
    private readonly CliHttpClientFactory _cliHttpClientFactory;
    private readonly ICancellationTokenProvider _cancellationTokenProvider;
    private readonly IRemoteServiceExceptionHandler _remoteServiceExceptionHandler;
    private readonly ILogger<AppService> _logger;

    public AppService(IAuthService authService,
        CliHttpClientFactory cliHttpClientFactory, ICancellationTokenProvider cancellationTokenProvider,
        ILogger<AppService> logger, IRemoteServiceExceptionHandler remoteServiceExceptionHandler)
    {
        _authService = authService;
        _cliHttpClientFactory = cliHttpClientFactory;
        _cancellationTokenProvider = cancellationTokenProvider;
        _logger = logger;
        _remoteServiceExceptionHandler = remoteServiceExceptionHandler;
    }

    public async Task DeployAsync(DeployAppOptions options)
    {
        _logger.LogInformation("Deploying AeIndexer...");
        
        var token = await _authService.GetAccessTokenAsync(options.Network, options.AppId, options.Key);
        var url = $"{CliConsts.AeFinderEndpoints[options.Network].ApiEndpoint}api/apps/subscriptions";
        var client = _cliHttpClientFactory.CreateClient(token);

        var formDataContent = new MultipartFormDataContent();
        formDataContent.Add(new StringContent(await File.ReadAllTextAsync(options.Manifest)), "Manifest");
        formDataContent.Add(new StreamContent(new MemoryStream(await File.ReadAllBytesAsync(options.Code))), "Code", "code.dll");
        foreach (var path in options.Attachments)
        {
            formDataContent.Add(new StreamContent(new MemoryStream(await File.ReadAllBytesAsync(path))), "AttachmentList", Path.GetFileName(path));
        }

        using var response = await client.PostAsync(
            url,
            formDataContent,
            _cancellationTokenProvider.Token
        );

        await _remoteServiceExceptionHandler.EnsureSuccessfulHttpResponseAsync(response);

        var responseContent = await response.Content.ReadAsStringAsync();
        _logger.LogInformation("Deploy AeIndexer successfully. Version: {Version}", responseContent);
    }

    public async Task UpdateAsync(UpdateAppOptions options)
    {
        var token = await _authService.GetAccessTokenAsync(options.Network, options.AppId, options.Key);

        if (!options.Code.IsNullOrWhiteSpace())
        {
            await UpdateCodeAsync(options, token);
        }

        if (!options.Manifest.IsNullOrWhiteSpace())
        {
            await UpdateManifestAsync(options, token);
        }
    }

    private async Task UpdateCodeAsync(UpdateAppOptions options, string token)
    {
        _logger.LogInformation("Updating AeIndexer code...");
        
        var url =
            $"{CliConsts.AeFinderEndpoints[options.Network].ApiEndpoint}api/apps/subscriptions/code/{options.Version}";

        var client = _cliHttpClientFactory.CreateClient(token);

        var formDataContent = new MultipartFormDataContent();
        formDataContent.Add(new StreamContent(new MemoryStream(await File.ReadAllBytesAsync(options.Code))), "Code", "code.dll");

        if (options.DeleteAttachmentKeys != null && options.DeleteAttachmentKeys.Any())
        {
            formDataContent.Add(new StringContent(options.DeleteAttachmentKeys.JoinAsString(",")), "AttachmentDeleteFileKeyList");
        }

        if (options.Attachments != null)
        {
            foreach (var path in options.Attachments)
            {
                formDataContent.Add(new StreamContent(new MemoryStream(await File.ReadAllBytesAsync(path))),
                    "AttachmentList", Path.GetFileName(path));
            }
        }

        using var response = await client.PutAsync(
            url,
            formDataContent,
            _cancellationTokenProvider.Token
        );

        await _remoteServiceExceptionHandler.EnsureSuccessfulHttpResponseAsync(response);

        _logger.LogInformation("Update AeIndexer successfully.");
    }

    private async Task UpdateManifestAsync(UpdateAppOptions options, string token)
    {
        _logger.LogInformation("Updating AeIndexer manifest...");
        
        var url =
            $"{CliConsts.AeFinderEndpoints[options.Network].ApiEndpoint}api/apps/subscriptions/manifest/{options.Version}";

        var client = _cliHttpClientFactory.CreateClient(token);

        using var response = await client.PutAsync(
            url,
            new StringContent(await File.ReadAllTextAsync(options.Manifest), Encoding.UTF8, MimeTypes.Application.Json),
            _cancellationTokenProvider.Token
        );

        await _remoteServiceExceptionHandler.EnsureSuccessfulHttpResponseAsync(response);

        _logger.LogInformation("Update manifest successfully.");
    }
}