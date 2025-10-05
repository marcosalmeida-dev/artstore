using ArtStore.Application.Features.Products.Services;
using ArtStore.Shared.Interfaces.Command;
using ArtStore.Shared.Interfaces.Query;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ArtStore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped<UserProfileStateService>();

        // Add HybridCache
        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(30),
                LocalCacheExpiration = TimeSpan.FromMinutes(5)
            };
        });

        // Add cache services
        services.AddScoped<IProductCacheService, ProductCacheService>();

        RegisterEventHandlers(services);
        RegisterQueryHandlers(services);
        RegisterCommands(services);

        return services;
    }

    static void RegisterEventHandlers(IServiceCollection services)
    {
        foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => p.IsClass && !p.IsAbstract && typeof(IDomainEventHandler).IsAssignableFrom(p)))
        {
            services.AddScoped(typeof(IDomainEventHandler), type);
        }
    }

    static void RegisterQueryHandlers(IServiceCollection services)
    {
        foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => p.IsClass && !p.IsAbstract && p.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))))
        {
            foreach (var interfaceType in type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
            {
                services.AddScoped(interfaceType, type);
            }
        }
    }

    static void RegisterCommands(IServiceCollection services)
    {
        foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => p.IsClass && !p.IsAbstract && p.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))))
        {
            foreach (var interfaceType in type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))
            {
                services.AddScoped(interfaceType, type);
            }
        }
    }

}