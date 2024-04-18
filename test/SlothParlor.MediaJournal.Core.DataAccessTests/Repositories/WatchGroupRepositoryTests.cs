using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SlothParlor.MediaJournal.Contracts.WatchGroup;
using SlothParlor.MediaJournal.Core.DataAccessTests.Collections;
using SlothParlor.MediaJournal.Core.DataAccessTests.Resources;
using SlothParlor.MediaJournal.Core.Journal;

namespace SlothParlor.MediaJournal.Core.DataAccessTests.Repositories;

[Collection(nameof(CommonTestResourcesCollection))]
public class WatchGroupRepositoryTests : IAsyncLifetime
{
    private readonly IMapper _mapper;
    private DbContainerDescriptor _db = null!;
    private CommonData _commonTestData = null!;
    private IWatchGroupRepositoryFactory _repositoryFactory = null!;

    public WatchGroupRepositoryTests(DefaultMapperFixture mapperFixture)
    {
        _mapper = mapperFixture.Mapper;
    }

    public async Task InitializeAsync()
    {
        _db = await DbContainerFactory.Default.CreateAsync();

        _repositoryFactory = new WatchGroupRepositoryFactory(_db.AppDbContext, _mapper);

        _commonTestData = await CommonData.Create(_db.AppDbContext);
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
    }

    [Fact]
    public async Task Repository_CreateEmpty()
    {
        // Arrange
        var plannedUserId = _commonTestData.DefaultUser.UserId;
        var repo = _repositoryFactory.Create(plannedUserId);

        var plannedProperties = new WatchGroupInput()
        {
            DisplayName = $"NewWatchGroup-{Guid.NewGuid()}",
        };

        // Act
        var watchGroup = await repo.CreateAsync(plannedProperties);

        // Assert
        Assert.True(watchGroup.WatchGroupId > 0, "Assigned non-zero ID by db.");
        Assert.Equal(plannedProperties.DisplayName, watchGroup.DisplayName);
        Assert.NotNull(watchGroup.Participants);
        Assert.Single(watchGroup.Participants);
        
        var participant = watchGroup.Participants.First();
        Assert.Equal(_commonTestData.DefaultUser.UserId, participant.UserId);
    }

    [Fact]
    public async Task Repository_Get()
    {
        // Arrange
        var repo = _repositoryFactory.Create(_commonTestData.DefaultUser.UserId);

        // Act
        var watchGroups = await repo.GetAsync();

        // Assert
        Assert.NotEmpty(watchGroups);
        Assert.Contains(watchGroups, wg => wg.WatchGroupId == _commonTestData.DefaultWatchGroup.WatchGroupId);
    }

    [Fact]
    public async Task Repository_Update()
    {
        // Arrange
        var repo = _repositoryFactory.Create(_commonTestData.DefaultUser.UserId);
        var defaultWatchGroup = _commonTestData.DefaultWatchGroup;
        var properties = new WatchGroupInput()
        {
            DisplayName = $"RenamedWatchGroup-{Guid.NewGuid()}",
        };

        // Act
        var updatedWatchGroup = await repo.UpdateAsync(
            defaultWatchGroup.WatchGroupId,
            properties);

        // Assert
        Assert.Equal(defaultWatchGroup.WatchGroupId, updatedWatchGroup.WatchGroupId);
        Assert.Equal(properties.DisplayName, updatedWatchGroup.DisplayName);
    }

    [Fact]
    public async Task Repository_Delete()
    {
        // Arrange
        var repo = _repositoryFactory.Create(_commonTestData.DefaultUser.UserId);
        var defaultWatchGroup = _commonTestData.DefaultWatchGroup;

        // Act
        await repo.DeleteAsync(defaultWatchGroup.WatchGroupId);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await _db.AppDbContext.WatchGroups
            .FirstAsync(wg => wg.WatchGroupId == defaultWatchGroup.WatchGroupId));
    }
}
