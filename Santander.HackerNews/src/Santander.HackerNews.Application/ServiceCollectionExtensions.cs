using Microsoft.Extensions.DependencyInjection;

namespace Santander.HackerNews.Application;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services
            .AddMediatR();
    }

    public static IServiceCollection AddMediatR(this IServiceCollection services) => services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
}
