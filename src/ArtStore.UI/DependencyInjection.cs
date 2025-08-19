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

        //services.AddAuthentication(options =>
        //{
        //    options.DefaultScheme = IdentityConstants.ApplicationScheme;
        //    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        //})
        //.AddIdentityCookies();

        //var connectionString = config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        //services.AddDbContext<ApplicationDbContext>(options =>
        //    options.UseSqlite(connectionString));
        //services.AddDatabaseDeveloperPageExceptionFilter();

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


        //        services.AddRazorComponents().AddInteractiveServerComponents().AddHubOptions(options=> options.MaximumReceiveMessageSize = 64 * 1024);
        //        services.AddCascadingAuthenticationState();
        //        services.AddScoped<IdentityUserAccessor>();
        //        services.AddScoped<IdentityRedirectManager>();
        //        services.AddMudServices(config =>
        //        {
        //            MudGlobal.InputDefaults.ShrinkLabel = true;
        //            //MudGlobal.InputDefaults.Variant = Variant.Outlined;
        //            //MudGlobal.ButtonDefaults.Variant = Variant.Outlined;
        //            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
        //            config.SnackbarConfiguration.NewestOnTop = false;
        //            config.SnackbarConfiguration.ShowCloseIcon = true;
        //            config.SnackbarConfiguration.VisibleStateDuration = 3000;
        //            config.SnackbarConfiguration.HideTransitionDuration = 500;
        //            config.SnackbarConfiguration.ShowTransitionDuration = 500;
        //            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;

        //            // we're currently planning on deprecating `PreventDuplicates`, at least to the end dev. however,
        //            // we may end up wanting to instead set it as internal because the docs project relies on it
        //            // to ensure that the Snackbar always allows duplicates. disabling the warning for now because
        //            // the project is set to treat warnings as errors.
        //#pragma warning disable 0618
        //            config.SnackbarConfiguration.PreventDuplicates = false;
        //#pragma warning restore 0618
        //        });
        //        services.AddMudPopoverService();
        //        services.AddMudBlazorSnackbar();
        //        services.AddMudBlazorDialog();
        //        services.AddHotKeys2();

        //        services.AddScoped<LocalizationCookiesMiddleware>()
        //            .Configure<RequestLocalizationOptions>(options =>
        //            {

        //                options.AddSupportedUICultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray());
        //                options.AddSupportedCultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray());
        //                options.DefaultRequestCulture = new RequestCulture(LocalizationConstants.DefaultLanguageCode);
        //                options.FallBackToParentUICultures = true;
        //            })
        //            .AddLocalization(options => options.ResourcesPath = LocalizationConstants.ResourcesPath);

        //        services.AddHangfire(configuration => configuration
        //                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        //                .UseSimpleAssemblyNameTypeSerializer()
        //                .UseRecommendedSerializerSettings()
        //                .UseInMemoryStorage())
        //            .AddHangfireServer()
        //            .AddMvc();

        //        services.AddControllers();

        //        services.AddScoped<IApplicationHubWrapper, ServerHubWrapper>()
        //            .AddSignalR(options=>options.MaximumReceiveMessageSize=64*1024);
        //        services.AddProblemDetails();
        //        services.AddHealthChecks();


        //        services.AddHttpClient("ocr", c =>
        //        {
        //            c.BaseAddress = new Uri("https://paddleocr.blazorserver.com/ocr/predict-by-url");
        //            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        });
        //        services.AddScoped<LocalTimeOffset>();
        //        services.AddScoped<HubClient>();
        //        services
        //            .AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>()
        //            .AddScoped<LayoutService>()
        //            .AddScoped<PermissionHelper>()
        //            .AddScoped<BlazorDownloadFileService>()
        //            .AddScoped<IUserPreferencesService, UserPreferencesService>()
        //            .AddScoped<IMenuService, MenuService>()
        //            .AddScoped<InMemoryNotificationService>()
        //            .AddScoped<INotificationService>(sp =>
        //            {
        //                var service = sp.GetRequiredService<InMemoryNotificationService>();
        //                service.Preload();
        //                return service;
        //            });


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

        //app.InitializeCacheFactory();
        //app.UseStatusCodePagesWithRedirects("/404");
        //app.MapHealthChecks("/health");
        //app.UseAuthentication();
        //app.UseAuthorization();
        //app.UseAntiforgery();
        //app.UseHttpsRedirection();
        //app.MapStaticAssets();

        //if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"Files")))
        //    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"Files"));

        //app.UseStaticFiles(new StaticFileOptions
        //{
        //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Files")),
        //    RequestPath = new PathString("/Files")
        //});

        //var localizationOptions = new RequestLocalizationOptions()
        //    .SetDefaultCulture(LocalizationConstants.DefaultLanguageCode)
        //    .AddSupportedCultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray())
        //    .AddSupportedUICultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray());

        //// Remove AcceptLanguageHeaderRequestCultureProvider to prevent the browser's Accept-Language header from taking effect
        //var acceptLanguageProvider = localizationOptions.RequestCultureProviders
        //    .OfType<AcceptLanguageHeaderRequestCultureProvider>()
        //    .FirstOrDefault();
        //if (acceptLanguageProvider != null)
        //{
        //    localizationOptions.RequestCultureProviders.Remove(acceptLanguageProvider);
        //}
        //app.UseRequestLocalization(localizationOptions);
        //app.UseMiddleware<LocalizationCookiesMiddleware>();
        //app.UseExceptionHandler();
        //app.UseHangfireDashboard("/jobs", new DashboardOptions
        //{
        //    Authorization = new[] { new HangfireDashboardAuthorizationFilter() },
        //    AsyncAuthorization = new[] { new HangfireDashboardAsyncAuthorizationFilter() }
        //});
        //app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
        //app.MapHub<ServerHub>(ISignalRHub.Url);

        ////QuestPDF License configuration
        //Settings.License = LicenseType.Community;

        //// Add additional endpoints required by the Identity /Account Razor components.
        //app.MapAdditionalIdentityEndpoints();
        //app.UseForwardedHeaders();
        //app.UseWebSockets(new WebSocketOptions()
        //{ // We obviously need this
        //    KeepAliveInterval = TimeSpan.FromSeconds(30), // Just in case
        //});

        //// Ensure database is created
        //using (var scope = app.Services.CreateScope())
        //{
        //    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        //    context.Database.EnsureCreated();
        //}

        return app;
    }
}
