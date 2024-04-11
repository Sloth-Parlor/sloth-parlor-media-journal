using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SlothParlor.MediaJournal.Core.DataAccessTests.Collections;
using SlothParlor.MediaJournal.Data;
using Testcontainers.PostgreSql;

namespace SlothParlor.MediaJournal.Core.DataAccessTests.Resources;

public class ApplicationTestDbFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container;
    
    private IServiceProvider? _dbServiceProvider;
    private MediaJournalDbContext? _dbContext;

    public ApplicationTestDbFixture(StaticResourcesFixture staticResources)
    {
        var dbScriptsHostDirectory = Path.Combine(
            staticResources.PostgresInfrastructureDirectory.FullName,
            @"init.d\");

        var containerBuilder = new PostgreSqlBuilder()
            .WithImage(Constants.PostgresImage)
            .WithEnvironment("APPUSER_PASSWORD", "test")
            .WithBindMount(dbScriptsHostDirectory, "/docker-entrypoint-initdb.d/")
            .WithAutoRemove(true)
            .WithCleanUp(true);

        _container = containerBuilder.Build();
    }

    public PostgreSqlContainer Container => _container;

    public IServiceProvider DbServiceProvider => _dbServiceProvider 
        ?? throw new InvalidOperationException($"{nameof(ApplicationTestDbFixture)} not initialized.");

    public MediaJournalDbContext DbContext => _dbContext
        ?? throw new InvalidOperationException($"{nameof(ApplicationTestDbFixture)} not initialized.");

    public string PostgresUserConnectionString => _container.GetConnectionString();

    public string AppUserConnectionString { get; private set; }

    public async Task InitializeAsync()
    {
        var serviceCollection = new ServiceCollection();

        await _container.StartAsync();

        AppUserConnectionString = CreateAppUserConnectionString();

        serviceCollection
            .AddEntityFrameworkNpgsql()
            .AddDbContext<MediaJournalDbContext>((options) => options
                .UseNpgsql(AppUserConnectionString)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors());

        _dbServiceProvider = serviceCollection.BuildServiceProvider();
        _dbContext = _dbServiceProvider.GetRequiredService<MediaJournalDbContext>();

        await _dbContext.Database.MigrateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_dbContext is not null)
        {
            var connection = _dbContext.Database.GetDbConnection();

            await Task.WhenAll(
                connection.CloseAsync(),
                _dbContext.Database.CloseConnectionAsync());

            await _dbContext.DisposeAsync();
        }

        await _container.DisposeAsync();
    }

    Task IAsyncLifetime.DisposeAsync() => DisposeAsync().AsTask();

    private string CreateAppUserConnectionString()
    {
        var properties = new Dictionary<string, string>
        {
            ["Host"] = _container.Hostname,
            ["Port"] = _container.GetMappedPublicPort(PostgreSqlBuilder.PostgreSqlPort).ToString(),
            ["Database"] = "sp_media_journal",
            ["Username"] = "appuser",
            ["Password"] = "test",
        };

        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }
}
