using System.IdentityModel.Tokens.Jwt;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using SlothParlor.MediaJounal.WebApp.Components;

// Must be set before OpenIdConnectOptions are configured
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

var builder = WebApplication.CreateBuilder(args);

// Configure CORS
builder.Services.AddCors(options =>
{
    var trustedOrigins = new List<string>();
    
    var applicationOrigins = builder.Configuration
        .GetSection("Cors:ApplicationOrigins")
        .Get<string[]>();

    if (applicationOrigins is null || applicationOrigins.Length == 0)
    {
        throw new ArgumentException("ApplicationOrigins must be set in the configuration.", nameof(applicationOrigins));
    }

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

// Configure Identity
var msGraphConfigSection = builder.Configuration.GetSection("MicrosoftGraphApi");
var msGraphScopes = msGraphConfigSection["Scopes"]?.Split(' ');

builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, Constants.AzureAdB2C)
    .EnableTokenAcquisitionToCallDownstreamApi(msGraphScopes)
        .AddDownstreamApi("MicrosoftGraphApi", msGraphConfigSection)
        .AddInMemoryTokenCaches();

builder.Services
    .AddRazorPages()
    .AddMicrosoftIdentityUI();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
        .AddMicrosoftIdentityConsentHandler()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

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
