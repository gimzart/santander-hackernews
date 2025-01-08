using Microsoft.Extensions.DependencyInjection;
using Santander.HackerNews.Application.ExternalServices;
using Santander.HackerNews.Infrastructure.ExternalServices;

namespace Santander.HackerNews.Infrastructure;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddHttpClient<IFirebaseHackerNewsExternalService, FirebaseHackerNewsExternalService>(client =>
        {
            client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
        });

        return services;
    }
}
