using System.Reflection;
using ArtStore.Application.Interfaces.Services;
using ArtStore.Domain.Common.Events.Dispatcher;
using ArtStore.Domain.Identity;
using ArtStore.Infrastructure.Configurations;
using ArtStore.Infrastructure.Constants.ClaimTypes;
using ArtStore.Infrastructure.Constants.Database;
using ArtStore.Infrastructure.Constants.Role;
using ArtStore.Infrastructure.Constants.User;
using ArtStore.Infrastructure.PermissionSet;
using ArtStore.Infrastructure.Persistence.Interceptors;
using ArtStore.Infrastructure.Services.MultiTenant;
using ArtStore.Infrastructure.Services.Storage;
using ArtStore.Shared.Interfaces.MultiTenant;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace ArtStore.Infrastructure;
public static class DependencyInjection
{
    private const string IDENTITY_SETTINGS_KEY = "IdentitySettings";
    private const string APP_CONFIGURATION_SETTINGS_KEY = "AppConfigurationSettings";
    private const string DATABASE_SETTINGS_KEY = "DatabaseSettings";
    private const string USE_IN_MEMORY_DATABASE_KEY = "UseInMemoryDatabase";
    private const string IN_MEMORY_DATABASE_NAME = "ArtStoreDb";
    private const string LOGIN_PATH = "/pages/authentication/login";
    private const int DEFAULT_LOCKOUT_TIME_SPAN_MINUTES = 5;
    private const int MAX_FAILED_ACCESS_ATTEMPTS = 5;

    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSettings(configuration)
            .AddDatabase(configuration)
            .AddServices();

        services
            .AddAuthenticationService(configuration)
            .AddCurrentUserServices();

        services.AddSingleton<IUsersStateContainer, UsersStateContainer>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }

    private static IServiceCollection AddSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<IdentitySettings>(configuration.GetSection(IDENTITY_SETTINGS_KEY))
            .AddSingleton(s => s.GetRequiredService<IOptions<IdentitySettings>>().Value)
            .AddSingleton<IIdentitySettings>(s => s.GetRequiredService<IOptions<IdentitySettings>>().Value);

        services.Configure<AppConfigurationSettings>(configuration.GetSection(APP_CONFIGURATION_SETTINGS_KEY))
            .AddSingleton(s => s.GetRequiredService<IOptions<AppConfigurationSettings>>().Value)
            .AddSingleton<IApplicationSettings>(s => s.GetRequiredService<IOptions<AppConfigurationSettings>>().Value);

        services.Configure<DatabaseSettings>(configuration.GetSection(DATABASE_SETTINGS_KEY))
            .AddSingleton(s => s.GetRequiredService<IOptions<DatabaseSettings>>().Value);
        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        if (configuration.GetValue<bool>(USE_IN_MEMORY_DATABASE_KEY))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(IN_MEMORY_DATABASE_NAME);
                options.EnableSensitiveDataLogging();
            });
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>((p, m) =>
            {
                var databaseSettings = p.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                m.AddInterceptors(p.GetServices<ISaveChangesInterceptor>());
                m.UseDatabase(databaseSettings.DBProvider, databaseSettings.ConnectionString);
            });
        }

        services.AddScoped<IDbContextFactory<ApplicationDbContext>, BlazorContextFactory<ApplicationDbContext>>();
        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());
        services.AddScoped<ApplicationDbContextInitializer>();

        return services;
    }

    private static DbContextOptionsBuilder UseDatabase(this DbContextOptionsBuilder builder, string dbProvider,
        string connectionString)
    {
        switch (dbProvider.ToLowerInvariant())
        {
            case DbProviderKeys.SqlServer:
                return builder.UseSqlServer(connectionString);

            case DbProviderKeys.SqLite:
                return builder.UseSqlite(connectionString);

            default:
                throw new InvalidOperationException($"DB Provider {dbProvider} is not supported.");
        }
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<PicklistService>()
            .AddSingleton<IPicklistService>(sp =>
            {
                var service = sp.GetRequiredService<PicklistService>();
                service.Initialize();
                return service;
            });

        services.AddSingleton<TenantService>()
            .AddSingleton<ITenantService>(sp =>
            {
                var service = sp.GetRequiredService<TenantService>();
                service.Initialize();
                return service;
            });
        services.AddSingleton<UserService>()
            .AddSingleton<IUserService>(sp =>
            {
                var service = sp.GetRequiredService<UserService>();
                service.Initialize();
                return service;
            });

        services.AddSingleton<RoleService>()
            .AddSingleton<IRoleService>(sp =>
            {
                var service = sp.GetRequiredService<RoleService>();
                service.Initialize();
                return service;
            });
        return services
            .AddScoped<IValidationService, ValidationService>()
            .AddScoped<IDateTime, DateTimeService>()
            .AddScoped<IPDFService, PDFService>()
            .AddStorageService();
    }

    private static IServiceCollection AddStorageService(this IServiceCollection services)
    {
        services.AddSingleton<IBlobStorageService>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var useLocalStorage = configuration.GetValue<bool>("UseLocalFileStorage", true);

            if (useLocalStorage)
            {
                var logger = sp.GetRequiredService<ILogger<LocalFileStorageService>>();
                return new LocalFileStorageService(configuration, logger);
            }
            else
            {
                var logger = sp.GetRequiredService<ILogger<AzureBlobStorageService>>();
                return new AzureBlobStorageService(configuration, logger);
            }
        });

        return services;
    }

    private static IServiceCollection AddAuthenticationService(this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddScoped<IUserStore<ApplicationUser>, MultiTenantUserStore>();
        services.AddScoped<UserManager<ApplicationUser>, MultiTenantUserManager>();
        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddClaimsPrincipalFactory<MultiTenantUserClaimsPrincipalFactory>()
            .AddDefaultTokenProviders();

        // Add the custom role validator MultiTenantRoleValidator to override the default validation logic.
        // Ensures role names are unique within each tenant.
        services.AddScoped<IRoleValidator<ApplicationRole>, MultiTenantRoleValidator>();

        // Find the default RoleValidator<ApplicationRole> registration in the service collection.
        var defaultRoleValidator = services.FirstOrDefault(descriptor => descriptor.ImplementationType == typeof(RoleValidator<ApplicationRole>));

        // If the default role validator is found, remove it to ensure only MultiTenantRoleValidator is used.
        if (defaultRoleValidator != null)
        {
            services.Remove(defaultRoleValidator);
        }
        services.Configure<IdentityOptions>(options =>
        {
            var identitySettings = configuration.GetRequiredSection(IDENTITY_SETTINGS_KEY).Get<IdentitySettings>();
            identitySettings = identitySettings ?? new IdentitySettings();
            // Password settings
            options.Password.RequireDigit = identitySettings.RequireDigit;
            options.Password.RequiredLength = identitySettings.RequiredLength;
            options.Password.RequireNonAlphanumeric = identitySettings.RequireNonAlphanumeric;
            options.Password.RequireUppercase = identitySettings.RequireUpperCase;
            options.Password.RequireLowercase = identitySettings.RequireLowerCase;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(DEFAULT_LOCKOUT_TIME_SPAN_MINUTES);
            options.Lockout.MaxFailedAccessAttempts = MAX_FAILED_ACCESS_ATTEMPTS;
            options.Lockout.AllowedForNewUsers = true;

            // Default SignIn settings.
            options.SignIn.RequireConfirmedEmail = true;
            options.SignIn.RequireConfirmedPhoneNumber = false;
            options.SignIn.RequireConfirmedAccount = true;

            // User settings
            options.User.RequireUniqueEmail = true;
            //options.Tokens.EmailConfirmationTokenProvider = "Email";

        });

        services.AddScoped<IIdentityService, IdentityService>()
            .AddAuthorizationCore(options =>
            {
                options.AddPolicy("CanPurge", policy => policy.RequireUserName(UserName.Administrator));
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole(RoleName.Admin));
                // Here I stored necessary permissions/roles in a constant
                foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c =>
                             c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
                {
                    var propertyValue = prop.GetValue(null);
                    if (propertyValue is not null)
                    {
                        options.AddPolicy((string)propertyValue,
                            policy => policy.RequireClaim(ApplicationClaimTypes.Permission, (string)propertyValue));
                    }
                }
            })
            .AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = configuration.GetValue<string>("Authentication:Microsoft:ClientId") ?? string.Empty;
                microsoftOptions.ClientSecret = configuration.GetValue<string>("Authentication:Microsoft:ClientSecret") ?? string.Empty;
                //microsoftOptions.CallbackPath = new PathString("/pages/authentication/ExternalLogin"); # dotn't set this parameter!!
            })
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = configuration.GetValue<string>("Authentication:Google:ClientId") ?? string.Empty;
                googleOptions.ClientSecret = configuration.GetValue<string>("Authentication:Google:ClientSecret") ?? string.Empty; ;
            }
            )
            .AddIdentityCookies(options => { });

        services.ConfigureApplicationCookie(options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromDays(15);
            options.SlidingExpiration = true;
            //options.SessionStore = new MemoryCacheTicketStore();
            options.LoginPath = LOGIN_PATH;
        });
        services.AddDataProtection().PersistKeysToDbContext<ApplicationDbContext>();

        return services;
    }

    private static IServiceCollection AddCurrentUserServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();
        services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
        services.AddScoped<ICurrentUserContextSetter, CurrentUserContextSetter>();
        return services;
    }
}