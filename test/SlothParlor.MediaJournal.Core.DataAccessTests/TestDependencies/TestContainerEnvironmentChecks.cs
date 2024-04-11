using DotNet.Testcontainers.Containers;
using Npgsql;
using SlothParlor.MediaJournal.Core.DataAccessTests.Collections;
using SlothParlor.MediaJournal.Core.DataAccessTests.Resources;
using System.Data;
using Testcontainers.PostgreSql;

namespace SlothParlor.MediaJournal.Core.DataAccessTests.TestDependencies;

[Collection(nameof(EmptyPostgresDbCollection))]
public class TestContainerEnvironmentChecks
{
    private readonly PostgreSqlContainer _container;

    public TestContainerEnvironmentChecks(EmptyPostgresDbFixture dbFixture)
    {
        _container = dbFixture.Container;
    }

    [Fact]
    public void TestContainer_ContainerStarts()
    {
        // Assert
        Assert.Equal(TestcontainersStates.Running, _container.State);
    }

    [Fact]
    public async Task TestContainer_DbConnectionSucceeds()
    {
        // Arrange
        var connectionString = _container.GetConnectionString();

        // Act
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        // Assert
        Assert.Equal(ConnectionState.Open, connection.State);
    }
}
