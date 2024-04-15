
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SlothParlor.MediaJournal.Data;
using Testcontainers.PostgreSql;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace SlothParlor.MediaJournal.Core.DataAccessTests.Resources;

public class DbContainerFactory
{
    private static DbContainerFactory? _defaultInstance;

    private readonly PostgreSqlBuilder _containerBuilder;
    private readonly IMessageSink? _diagnosticMessageSink;

    public DbContainerFactory(
        string appuserPassword = "test",
        IMessageSink? diagnosticMessageSink = null
    )
    {
        this._diagnosticMessageSink = diagnosticMessageSink;
        var postgresInfraPath = GetPostgresInfraPath();
        var hostDirectories = new {
            initScripts = Path.Combine(postgresInfraPath, "init.d"),
        };

        _containerBuilder = new PostgreSqlBuilder()
            .WithImage(Constants.PostgresImage)
            .WithEnvironment("APPUSER_PASSWORD", appuserPassword)
            .WithBindMount(hostDirectories.initScripts, "/docker-entrypoint-initdb.d/");
    }

    public async Task<DbContainerDescriptor> CreateAsync()
    {
        var stats = new Dictionary<string, TimeSpan>();
        var taskStopwatch = new Stopwatch();
        var operationStopwatch = new Stopwatch();

        taskStopwatch.Start();

        var container = _containerBuilder.Build();
        
        operationStopwatch.Start();
        await container.StartAsync();
        stats.Add("ContainerStart", operationStopwatch.Elapsed);

        var appUserConnectionString = CreateAppUserConnectionString(container);

        operationStopwatch.Restart();
        var appDbContext = new MediaJournalDbContext(CreateDbContextOptions(appUserConnectionString));
        stats.Add("DbContextCreate", operationStopwatch.Elapsed);

        operationStopwatch.Restart();
        await appDbContext.Database.MigrateAsync();
        stats.Add("EfMigrate", operationStopwatch.Elapsed);
        
        taskStopwatch.Stop();
        stats.Add("TotalDbCreation", taskStopwatch.Elapsed);

        DumpStats(stats);

        return new DbContainerDescriptor
        {
            Container = container,
            AppDbContext = appDbContext,
            PostgresUserConnectionString = container.GetConnectionString(),
            AppUserConnectionString = appUserConnectionString,
            Stats = stats,
        };
    }

    private void DumpStats(IDictionary<string, TimeSpan> stats)
    {
        if (_diagnosticMessageSink is null)
        {
            return;
        }

        var statsJson = JsonSerializer.Serialize(stats, options: new() { WriteIndented = true });
        _diagnosticMessageSink.OnMessage(new DiagnosticMessage("DB TestContainer creation stats:\n{0}", statsJson));
    }

    private static string GetPostgresInfraPath()
    {
        var executingAssemblyPath = Assembly.GetExecutingAssembly()?.Location
            ?? throw new InvalidOperationException("Could not establish fully qualified path to test assembly.");

        var baseDir = Path.GetDirectoryName(executingAssemblyPath)
            ?? throw new InvalidOperationException("Could not establish base directory for test assembly.");

        return Path.Combine(
            baseDir,
            Constants.PostgresInfrastructureDirectory);
    }

    private static string CreateAppUserConnectionString(PostgreSqlContainer _container)
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

    public static DbContainerFactory Default 
    {
        get => _defaultInstance ??= new DbContainerFactory();
    }

    private static DbContextOptions<MediaJournalDbContext> CreateDbContextOptions(string connectionString)
    {
        return new DbContextOptionsBuilder<MediaJournalDbContext>()
            .UseNpgsql(connectionString)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .Options;
    }
}