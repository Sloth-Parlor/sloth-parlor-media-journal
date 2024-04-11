using SlothParlor.MediaJournal.Core.DataAccessTests.Collections;
using SlothParlor.MediaJournal.Core.DataAccessTests.Resources;

namespace SlothParlor.MediaJournal.Core.DataAccessTests.TestDependencies;

[Collection(nameof(CommonTestResourcesCollection))]
public class TestResourcesTests
{
    private readonly StaticResourcesFixture _staticResources;

    public TestResourcesTests(StaticResourcesFixture staticResources)
    {
        _staticResources = staticResources;
    }

    [Fact]
    public void TestResourcesTests_ExpectedContents()
    {
        Assert.True(_staticResources.BasePath.Exists);

        Assert.True(_staticResources.PostgresInfrastructureDirectory.Exists);
        var postgresTestInfraChildDirectories = _staticResources.PostgresInfrastructureDirectory
            .EnumerateDirectories()
            .Select(di => di.Name);

        Assert.Contains("init.d", postgresTestInfraChildDirectories);
    }
}
