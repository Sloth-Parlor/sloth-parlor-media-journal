using SlothParlor.MediaJournal.Core.DataAccessTests.Collections;
using SlothParlor.MediaJournal.Core.DataAccessTests.Resources;
using SlothParlor.MediaJournal.Data;

namespace SlothParlor.MediaJournal.Core.DataAccessTests.TestDependencies;

[Collection(nameof(CommonTestResourcesCollection))]
public class TestAppDbContextTests : IClassFixture<ApplicationTestDbFixture>
{
    private readonly MediaJournalDbContext _dbContext;

    public TestAppDbContextTests(ApplicationTestDbFixture appDbFixture)
    {
        _dbContext = appDbFixture.DbContext;
    }

    [Fact]
    public async Task TestAppDbContext_CanConnect()
    {
        var result = await _dbContext.Database.CanConnectAsync();

        Assert.True(result);
    }
}
