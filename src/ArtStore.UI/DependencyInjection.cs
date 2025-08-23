using ArtStore.Application.Common.Interfaces;
using ArtStore.Application.Hubs;
using ArtStore.Domain.Identity;
using ArtStore.Infrastructure.Constants.Localization;
using ArtStore.Infrastructure.Persistence;
using ArtStore.UI.Client.Services;
using ArtStore.UI.Components;
using ArtStore.UI.Components.Account;
using ArtStore.UI.Hubs;
using ArtStore.UI.Middlewares;
using Microsoft.AspNetCore.Localization;
using MudBlazor.Services;
using MudExtensions.Services;


namespace ArtStore.UI;

/// <summary>
/// Provides dependency injection configuration for the server UI.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds server UI services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="config">The configuration.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddServerUI(this IServiceCollection services, IConfiguration config)
    {
        // Add MudBlazor services
        services.AddMudServices();
        services.AddMudExtensions();

        // Add services to the container.
        services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents()
            .AddAuthenticationStateSerialization();

        services.AddCascadingAuthenticationState();
        services.AddScoped<IdentityUserAccessor>();
        services.AddScoped<IdentityRedirectManager>();
        services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

        services.AddScoped<LocalizationCookiesMiddleware>()
            .Configure<RequestLocalizationOptions>(options =>
            {

                options.AddSupportedUICultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray());
                options.AddSupportedCultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray());
                options.DefaultRequestCulture = new RequestCulture(LocalizationConstants.DefaultLanguageCode);
                options.FallBackToParentUICultures = true;
            })
            .AddLocalization(options => options.ResourcesPath = LocalizationConstants.ResourcesPath);

        services.AddScoped<IApplicationHubWrapper, ServerHubWrapper>()
            .AddSignalR(options => options.MaximumReceiveMessageSize = 64 * 1024);
        services.AddProblemDetails();
        services.AddHealthChecks();

        var baseUrl = config["BaseUrl"];
        if (string.IsNullOrEmpty(baseUrl))
        {
            throw new InvalidOperationException("BaseUrl configuration is missing or empty.");
        }
        var baseUrlAddress = new Uri(baseUrl);

        services.AddHttpClient<OrderService>(c =>
        {
            c.BaseAddress = baseUrlAddress;
        });
        services.AddHttpClient<ProductService>(c =>
        {
            c.BaseAddress = baseUrlAddress;
        });

        services.AddMemoryCache();

        services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.IncludeFields = true; // If fields are used
                });

        return services;
    }

    /// <summary>
    /// Configures the server pipeline.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <param name="config">The configuration.</param>
    /// <returns>The configured web application.</returns>
    public static WebApplication ConfigureServer(this WebApplication app, IConfiguration config)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.MapControllers();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(ArtStore.UI.Client._Imports).Assembly);

        app.MapHub<OrderManagementHub>(IOrderManagementHub.Url);

        // Add additional endpoints required by the Identity /Account Razor components.
        app.MapAdditionalIdentityEndpoints();

        // Ensure database is created
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error", true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
       
        return app;
    }
}
