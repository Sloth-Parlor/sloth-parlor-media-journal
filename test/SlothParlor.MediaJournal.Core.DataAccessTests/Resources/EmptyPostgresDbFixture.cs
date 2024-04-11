using SlothParlor.MediaJournal.Core.DataAccessTests.Collections;
using Testcontainers.PostgreSql;

namespace SlothParlor.MediaJournal.Core.DataAccessTests.Resources;

public class EmptyPostgresDbFixture : IAsyncLifetime
{
    public EmptyPostgresDbFixture()
    {
        var builder = new PostgreSqlBuilder()
            .WithImage(Constants.PostgresImage);

        Container = builder.Build();
    }

    public PostgreSqlContainer Container { get; init; }

    public async Task InitializeAsync()
    {
        await Container.StartAsync();
    }

    public ValueTask DisposeAsync()
    {
        return Container.DisposeAsync();
    }

    Task IAsyncLifetime.DisposeAsync() => DisposeAsync().AsTask();
}
