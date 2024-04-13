using Microsoft.EntityFrameworkCore;
using SlothParlor.MediaJournal.Data;
using Testcontainers.PostgreSql;

namespace SlothParlor.MediaJournal.Core.DataAccessTests.Resources;

public class DbContainerDescriptor : IAsyncDisposable
{
    public required PostgreSqlContainer Container { get; init; }

    public required MediaJournalDbContext AppDbContext { get; init; }

    public required string PostgresUserConnectionString { get; init; }

    public required string AppUserConnectionString { get; init; }

    public Dictionary<string, TimeSpan>? Stats { get; init; }


    public async ValueTask DisposeAsync()
    {
        var connection = AppDbContext.Database.GetDbConnection();

        await Task.WhenAll(
            connection.CloseAsync(),
            AppDbContext.Database.CloseConnectionAsync());

        await Container.DisposeAsync();
    }
}
