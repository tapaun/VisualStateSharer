using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;
using VisualStateSharer.Api;
using VisualStateSharer.Configuration;
using VisualStateSharer.Interfaces;

namespace VisualStateSharer.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVisualStateSharer(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind configuration settings
        services.AddOptions<PastebinSettings>()
            .Bind(configuration.GetSection("Pastebin"))
            .ValidateOnStart();
            
        services.AddOptions<GyazoSettings>()
            .Bind(configuration.GetSection("Gyazo"))
            .ValidateOnStart();
        
        // Register Pastebin Refit client
        services.AddRefitClient<IPastebinApi>()
            .ConfigureHttpClient((sp, client) => 
            {
                var settings = sp.GetRequiredService<IOptions<PastebinSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(settings.Timeout);
            });
        
        // Register Gyazo Refit client
        services.AddRefitClient<IGyazoApi>()
            .ConfigureHttpClient((sp, client) => 
            {
                var settings = sp.GetRequiredService<IOptions<GyazoSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(settings.Timeout);
            });
        
        // Register PastebinApi service
        services.AddScoped<PastebinApi>(sp =>
        {
            var api = sp.GetRequiredService<IPastebinApi>();
            var settings = sp.GetRequiredService<IOptions<PastebinSettings>>().Value;
            var logger = sp.GetRequiredService<ILogger<PastebinApi>>();
            return new PastebinApi(api, settings.ApiKey, logger);
        });
        
        // Register GyazoApi service
        services.AddScoped<GyazoApi>(sp =>
        {
            var api = sp.GetRequiredService<IGyazoApi>();
            var settings = sp.GetRequiredService<IOptions<GyazoSettings>>().Value;
            var logger = sp.GetRequiredService<ILogger<GyazoApi>>();
            return new GyazoApi(api, settings.AccessToken, logger);
        });
        
        return services;
    }
}
