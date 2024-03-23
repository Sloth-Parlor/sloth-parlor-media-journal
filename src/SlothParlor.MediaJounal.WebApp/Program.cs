using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using SlothParlor.MediaJounal.WebApp.Components;

// Must be set before OpenIdConnectOptions are configured
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

var builder = WebApplication.CreateBuilder(args);

var applicationOrigins = builder.Configuration
    .GetSection("Cors:ApplicationOrigins")
    .Get<string[]>() ?? [];

if (applicationOrigins.Length == 0)
{
    throw new ArgumentException("ApplicationOrigins must be set in the configuration.", nameof(applicationOrigins));
}

// Additional configuration sources
if (builder.Configuration.GetValue<Uri>("AzureKeyVault:Uri") is Uri keyVaultUri)
{
    builder.Configuration
        .AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
}

// Configure CORS
builder.Services.AddCors(options =>
{
    var trustedOrigins = new List<string>();

    trustedOrigins.AddRange(applicationOrigins);
    trustedOrigins.AddRange(builder.Configuration
        .GetSection("Cors:IdentityOrigins")
        .Get<string[]>() ?? []);

    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.WithOrigins([.. trustedOrigins])
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Configure proxy
if (builder.Configuration.GetValue<bool>("UseForwardedHeaders"))
{
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
        options.RequireHeaderSymmetry = false;

        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();

        options.AllowedHosts.Clear();
        foreach (var origin in applicationOrigins)
        {
            var uri = new Uri(origin);
            var host = uri.Host;
            options.AllowedHosts.Add(host);
        }
    });
}

// Configure Identity
var msGraphConfigSection = builder.Configuration.GetSection("MicrosoftGraphApi");
var msGraphScopes = msGraphConfigSection["Scopes"]?.Split(' ');

builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, Constants.AzureAdB2C)
    .EnableTokenAcquisitionToCallDownstreamApi(msGraphScopes)
        .AddDownstreamApi("MicrosoftGraphApi", msGraphConfigSection)
        .AddInMemoryTokenCaches();

builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    var redirectToIdentityProvider = options.Events.OnRedirectToIdentityProvider;

    options.Events.OnRedirectToIdentityProvider = async context =>
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

        await redirectToIdentityProvider(context);

        var protocolMessageData = new
        {
            context.ProtocolMessage.DomainHint,
            context.ProtocolMessage.IdentityProvider,
            context.ProtocolMessage.IssuerAddress,
            context.ProtocolMessage.LoginHint,
            context.ProtocolMessage.RedirectUri,
        };

        var protocolMessageDataJson = JsonSerializer.Serialize(protocolMessageData, options: new() { WriteIndented = true });
        logger.LogInformation("OpenIdConnect redirect message data (json):\n{Json}", protocolMessageDataJson);
    };
});

// Configure webapp services
builder.Services
    .AddRazorPages()
    .AddMicrosoftIdentityUI();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
        .AddMicrosoftIdentityConsentHandler()
    .AddInteractiveWebAssemblyComponents();

builder.Services
    .AddCascadingAuthenticationState();

var app = builder.Build();

app.Use(async (context, next) =>
{
    var requestData = new
    {
        context.Request.IsHttps,
        context.Request.Scheme, 
        context.Request.Host,
        context.Request.Path,
        context.Request.PathBase,
        context.Request.Headers,
    };

    var requestDataJson = JsonSerializer.Serialize(requestData, options: new() { WriteIndented = true });
    app.Logger.LogDebug("initial request data (json):\n{Json}", requestDataJson);

    await next();
});

if (builder.Configuration.GetValue<bool>("UseForwardedHeaders"))
{
    app.UseForwardedHeaders();
}

if (app.Configuration["BasePath"] is string basePath && !string.IsNullOrWhiteSpace(basePath))
{
    app.UsePathBase(basePath);
}

app.Use(async (context, next) =>
{
    var requestData = new
    {
        context.Request.IsHttps,
        context.Request.Scheme,
        context.Request.Host,
        context.Request.Path,
        context.Request.PathBase,
        context.Request.Headers,
        Middleware = new string[] { "UseForwardedHeaders", "UsePathBase" },
    };

    var requestDataJson = JsonSerializer.Serialize(requestData, options: new() { WriteIndented = true });
    app.Logger.LogDebug("modified request data (json):\n{Json}", requestDataJson);

    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(SlothParlor.MediaJounal.WebApp.Client._Imports).Assembly);

app.Run();
