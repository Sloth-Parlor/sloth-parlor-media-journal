using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SlothParlor.MediaJournal.Contracts.MediaLog;
using SlothParlor.MediaJournal.Core.DataAccessTests.Collections;
using SlothParlor.MediaJournal.Core.DataAccessTests.Resources;
using SlothParlor.MediaJournal.Core.Journal;
using SlothParlor.MediaJournal.Data.Models;
using Xunit.Abstractions;

namespace SlothParlor.MediaJournal.Core.DataAccessTests.Repositories;

[Collection(nameof(CommonTestResourcesCollection))]
public class MediaLogRepositoryTests : IAsyncLifetime
{
    private readonly IMapper _mapper;

    private DbContainerDescriptor _db = null!;
    private CommonData _commonTestData = null!;
    private IMediaLogRepositoryFactory _repositoryFactory = null!;

    public MediaLogRepositoryTests(DefaultMapperFixture mapperFixture)
    {
        _mapper = mapperFixture.Mapper;
    }

    public async Task InitializeAsync()
    {
        _db = await DbContainerFactory.Default.CreateAsync();

        _repositoryFactory = new MediaLogRepositoryFactory(_db.AppDbContext, _mapper);

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
        var repo = _repositoryFactory.Create(_commonTestData.DefaultWatchGroup.WatchGroupId);

        var properties = new MediaLogInput()
        {
            DisplayName = $"NewMediaLog-{Guid.NewGuid()}",
        };

        // Act
        var mediaLog = await repo.CreateEmptyAsync(properties);

        // Assert
        Assert.True(mediaLog.MediaLogId > 0, "Assigned non-zero ID by db.");
        Assert.Equal(properties.DisplayName, mediaLog.DisplayName);
        Assert.Empty(mediaLog.LogEntries);
    }

    [Fact]
    public async Task Repository_Update()
    {
        // Arrange
        var repo = _repositoryFactory.Create(_commonTestData.DefaultWatchGroup.WatchGroupId);
        var defaultMediaLog = _commonTestData.DefaultMediaLog;
        var properties = new MediaLogInput()
        {
            DisplayName = $"RenamedMediaLog-{Guid.NewGuid()}",
        };

        // Act
        var updatedMediaLog = await repo.UpdateAsync(
            defaultMediaLog.MediaLogId,
            properties,
            async (entry) => await Task.WhenAll(
                entry.Navigation(nameof(MediaLog.LogEntries)).LoadAsync(),
                entry.Navigation(nameof(MediaLog.WatchGroup)).LoadAsync()));

        // Assert
        Assert.Equal(defaultMediaLog.MediaLogId, updatedMediaLog.MediaLogId);
        Assert.Equal(properties.DisplayName, updatedMediaLog.DisplayName);
        Assert.Equal(
            defaultMediaLog.LogEntries?.Select(_mapper.Map<EntryResult>) ?? [],
            updatedMediaLog.LogEntries);
    }

    [Fact]
    public async Task Repository_Delete()
    {
        // Arrange
        var repo = _repositoryFactory.Create(_commonTestData.DefaultWatchGroup.WatchGroupId);
        var defaultMediaLog = _commonTestData.DefaultMediaLog;
        var properties = new MediaLogInput()
        {
            DisplayName = $"RenamedMediaLog-{Guid.NewGuid()}",
        };

        // Act
        await repo.DeleteAsync(defaultMediaLog.MediaLogId);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await _db.AppDbContext.MediaLogs
            .FirstAsync(ml => ml.MediaLogId == defaultMediaLog.MediaLogId));
    }
}