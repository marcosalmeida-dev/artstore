using ArtStore.Application;
using ArtStore.Infrastructure;
using ArtStore.Infrastructure.Extensions;
using ArtStore.UI;

var builder = WebApplication.CreateBuilder(args);
builder.RegisterSerilog();
builder.WebHost.UseStaticWebAssets();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddServerUI(builder.Configuration);

var app = builder.Build();

app.ConfigureServer(builder.Configuration);

await app.InitializeDatabaseAsync().ConfigureAwait(false);

await app.RunAsync().ConfigureAwait(false);
