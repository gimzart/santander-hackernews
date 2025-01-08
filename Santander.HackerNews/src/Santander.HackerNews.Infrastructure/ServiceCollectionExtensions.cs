using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using Polly.Retry;
using Santander.HackerNews.Application.ExternalServices;
using Santander.HackerNews.Infrastructure.ExternalServices;

namespace Santander.HackerNews.Infrastructure;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services
            .AddHttpClient<IFirebaseHackerNewsExternalService, FirebaseHackerNewsExternalService>(client =>
            {
                client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
            })
            .AddPolicyHandler(GetRetryPolicy());

        return services;
    }

    static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 3));
    }
}
