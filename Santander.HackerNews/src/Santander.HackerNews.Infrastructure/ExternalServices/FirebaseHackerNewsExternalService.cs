using System.Text.Json;
using Microsoft.Extensions.Logging;
using Santander.HackerNews.Application.ExternalServices;
using Santander.HackerNews.DomainEntities;

namespace Santander.HackerNews.Infrastructure.ExternalServices;
internal class FirebaseHackerNewsExternalService : IFirebaseHackerNewsExternalService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FirebaseHackerNewsExternalService> _logger;
    private static SemaphoreSlim _semaphore = new SemaphoreSlim(500, 500);

    public FirebaseHackerNewsExternalService(HttpClient httpClient, ILogger<FirebaseHackerNewsExternalService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public Task<IEnumerable<int>?> GetBestStories(CancellationToken cancellationToken)
    {
        return GetAsync<IEnumerable<int>>("beststories.json", cancellationToken);
    }

    public async Task<Item?> GetStoryById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var item = await GetAsync<Item>($"item/{id}.json", cancellationToken);
            return item;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting item {id}", id);
            return null;
        }
    }

    protected async Task<TResult?> GetAsync<TResult>(string path, CancellationToken cancellationToken)
    {
        HttpResponseMessage? response;
        try
        {
            await _semaphore.WaitAsync(cancellationToken);
            response = await _httpClient.GetAsync(path, cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }

        if (response.IsSuccessStatusCode)
        {
            return await DesrializeContent<TResult>(response, cancellationToken);
        }
        else
        {
            throw new ApplicationException($"Error calling {path}");
        }
    }

    private static async Task<TResult?> DesrializeContent<TResult>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var value = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken);
        var result = JsonSerializer.Deserialize<TResult>(value);
        return result;
    }
}
