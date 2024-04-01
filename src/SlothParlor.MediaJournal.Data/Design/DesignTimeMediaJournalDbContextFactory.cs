using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.CommandLine.Parsing;
using System.Text.Json;

namespace SlothParlor.MediaJournal.Data.Design;

internal class DesignTimeMediaJournalDbContextFactory 
    : IDesignTimeDbContextFactory<MediaJournalDbContext>
{
    public MediaJournalDbContext CreateDbContext(string[] args)
    {
        var designTimeToolArgs = CommandLineStringSplitter.Instance
            .Split(Environment.CommandLine)
            .ToArray();

        DebugInfoToConsole(designTimeToolArgs, args);

        var optionsBuilder = new DbContextOptionsBuilder<MediaJournalDbContext>();

        if (!IsConnectionExpected(designTimeToolArgs, args))
        {
            Console.WriteLine($"Using a {nameof(MediaJournalDbContext)} configured with Npgsql, without a connection string.");
            optionsBuilder.UseNpgsql();
            return new MediaJournalDbContext(optionsBuilder.Options);
        }

        if (designTimeToolArgs.Contains("--connection"))
        {
            Console.WriteLine($"Using a {nameof(MediaJournalDbContext)} configured with Npgsql, but no configuration-time connection string; the --connection param will be used after the context is created (see https://github.com/dotnet/efcore/blob/v8.0.3/src/EFCore.Design/Design/Internal/MigrationsOperations.cs#L191-L218).");
            optionsBuilder.UseNpgsql();
            return new MediaJournalDbContext(optionsBuilder.Options);
        }

        IConfiguration configuration = BuildConfiguration(args);

        Console.WriteLine("Using connecting string from configuration.");
        if (!TryGetConnectionString(configuration, out var connectionStringFromConfiguration))
        {
            ArgumentNullException.ThrowIfNull(connectionStringFromConfiguration);
        }

        Console.WriteLine(JsonSerializer.Serialize(new { connectionStringFromConfiguration }, options: new() { WriteIndented = true }));
        optionsBuilder.UseNpgsql(connectionStringFromConfiguration);

        return new MediaJournalDbContext(optionsBuilder.Options);
    }

    private bool IsConnectionExpected(string[] designTimeToolArgs, string[] localArgs)
    {
        if (IsBundler(designTimeToolArgs))
        {
            // todo: Warn if a "--no-connect" local arg is passed; 
            // since the bundler needs to connect to the database
            // and will fail even with the --no-connect flag present
            return true;
        }

        if (designTimeToolArgs.Any(t => t == "--no-connect"))
        {
            return false;
        }

        return true;
    }

    private bool IsBundler(string[] designTimeToolArgs)
    {
        // Infer that we're running in a bundler if ef.dll is not in the args
        return !designTimeToolArgs.Any(t => t.Contains("ef.dll"));
    }

    private bool TryGetConnectionString(IConfiguration configuration, out string? connectionString)
    {
        connectionString = configuration.GetConnectionString("AppDb");

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            return true;
        }

        return false;
    }

    private static void DebugInfoToConsole(
        string[] designTimeToolArgs, 
        string[] args)
    {
        Console.WriteLine("----");
        Console.WriteLine(JsonSerializer.Serialize(new
        {
            EnvironmentVariables = new
            {
                PgParams = new Dictionary<string, object?>()
                {
                    [PgEnvVars.Host] = Environment.GetEnvironmentVariable(PgEnvVars.Host),
                    [PgEnvVars.Database] = Environment.GetEnvironmentVariable(PgEnvVars.Database),
                    [PgEnvVars.User] = Environment.GetEnvironmentVariable(PgEnvVars.User),
                },
                PasswordEnvVarIsSet = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(PgEnvVars.Password)),
            },
            Environment.CommandLine,
            designTimeToolArgs,
            LocalArgs = args,
        }, options: new() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        Console.WriteLine("----");
    }

    public static IConfiguration BuildConfiguration(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("efcore.design.json", optional: true)
            .AddUserSecrets<DesignTimeMediaJournalDbContextFactory>()
            .AddEnvironmentVariables()
            .AddCommandLine(args);

        return builder.Build();
    }
}

public static class PgEnvVars
{
    public const string Host = "PGHOST";
    public const string Database = "PGDATABASE";
    public const string User = "PGUSER";
    public const string Password = "PGPASSWORD";
}
