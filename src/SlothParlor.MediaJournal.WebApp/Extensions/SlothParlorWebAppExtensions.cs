using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;

namespace SlothParlor.MediaJournal.WebApp.Extensions;

public static class SlothParlorWebAppExtensions
{
    public const string AzureKeyVaultConfigSection = "AzureKeyVault";
    public const string AzureMonitorConfigSection = "AzureMonitor";
    public const string AspireDashboardConfigSection = "AspireDashboard";

    private static Func<IHostApplicationBuilder, Action<ResourceBuilder>> ConfigureApplicationResource = 
        (hostAppBuilder) =>
        {
            var webHostEnvApplicationName = hostAppBuilder.Environment.ApplicationName;

            return (r) => r.AddService(webHostEnvApplicationName);
        };

    public static WebApplicationBuilder AddKeyVaultConfigSourceIfConfigured(this WebApplicationBuilder builder)
    {
        if (builder.Configuration.GetValue<Uri>($"{AzureKeyVaultConfigSection}:Uri") is Uri keyVaultUri)
        {
            builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
        }

        return builder;
    }

    public static WebApplicationBuilder AddAppMonitoringIfConfigured(this WebApplicationBuilder builder)
    {
        if (builder.Configuration.GetSection(AzureMonitorConfigSection).Exists())
        {
            builder.AddAzureMonitor();
        }

        if (builder.Configuration.GetSection(AspireDashboardConfigSection).Exists())
        {
            builder.AddAspireMonitor();
        }

        return builder;
    }

    public static OpenTelemetryBuilder AddAzureMonitor(this WebApplicationBuilder appBuilder)
    {
        var configSection = appBuilder.Configuration
            .GetRequiredSection(AzureMonitorConfigSection);

        var otelBuilder = appBuilder.Services.AddOpenTelemetry().ConfigureResource(ConfigureApplicationResource(appBuilder));

        otelBuilder
            .UseAzureMonitor()
            .ConfigureResource(ConfigureApplicationResource(appBuilder));

        return otelBuilder;
    }

    public static OpenTelemetryBuilder AddAspireMonitor (this WebApplicationBuilder appBuilder)
    {
        var configSection = appBuilder.Configuration
            .GetRequiredSection(AspireDashboardConfigSection);

        appBuilder.Services.Configure<OtlpExporterOptions>(configSection.Bind);

        var otelBuilder = appBuilder.Services.AddOpenTelemetry();

        if (!configSection.Exists() || !(configSection.GetValue<bool>("Enabled") is true))
        {
            return otelBuilder;
        }

        otelBuilder.ConfigureResource(ConfigureApplicationResource(appBuilder));

        appBuilder.Logging.AddOpenTelemetry(options =>
        {
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;
            options.ParseStateValues = true;

            var resourceBuilder = ResourceBuilder.CreateDefault();
            var configureResource = ConfigureApplicationResource(appBuilder);
            configureResource(resourceBuilder);
            options.SetResourceBuilder(resourceBuilder);
        });

        appBuilder.Services.Configure<OpenTelemetryLoggerOptions>(options =>
        {
            options.AddOtlpExporter();
        });

        otelBuilder
            .WithMetrics(meterProvider => meterProvider.AddOtlpExporter())
            .WithTracing(tracerProvider => tracerProvider.AddOtlpExporter());

        return otelBuilder;
    }
}
