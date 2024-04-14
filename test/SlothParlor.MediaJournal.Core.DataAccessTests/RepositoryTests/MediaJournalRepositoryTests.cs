using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SlothParlor.MediaJournal.Contracts.MediaLog;
using SlothParlor.MediaJournal.Core.DataAccessTests.Collections;
using SlothParlor.MediaJournal.Core.DataAccessTests.Resources;
using SlothParlor.MediaJournal.Core.Journal;
using SlothParlor.MediaJournal.Data.Models;
using Xunit.Abstractions;

namespace SlothParlor.MediaJournal.Core.DataAccessTests.RepositoryTests;

[Collection(nameof(CommonTestResourcesCollection))]
public class MediaJournalRepositoryTests : IAsyncLifetime
{
    private readonly ITestOutputHelper _output;
    private readonly IMapper _mapper;

    private DbContainerDescriptor _db = null!;
    private CommonTestData _commonTestData = null!;
    private IMediaLogRepositoryFactory _repositoryFactory = null!;

    public MediaJournalRepositoryTests(ITestOutputHelper output, ApplicationMapperFixture mapperFixture)
    {
        _output = output;
        _mapper = mapperFixture.Mapper;
    }

    public async Task InitializeAsync()
    {
        _db = await DbContainerFactory.Default.CreateAsync();

        _repositoryFactory = new MediaLogRepositoryFactory(_db.AppDbContext, _mapper);

        _commonTestData = await CreateCommonTestDataAsync();
    }


    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
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
            DisplayName = $"NewMediaLog-{Guid.NewGuid()}",
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

    private async Task<CommonTestData> CreateCommonTestDataAsync()
    {
        User testuser1 = new()
        {
            UserId = Guid.NewGuid().ToString(),
            Email = "testuser1@dev.slothparlor.com",
        };

        await _db.AppDbContext.AddAsync(testuser1);

        WatchGroup testUser1DefaultWatchGroup = new()
        {
            DisplayName = "Default",
            Owners = [testuser1],
        };

        var watchGroupEntityResult = await _db.AppDbContext.AddAsync(testUser1DefaultWatchGroup);

        WatchGroupParticipant[] Participants = [
            new()
            {
                UserId = testuser1.UserId,
                WatchGroup = testUser1DefaultWatchGroup,
                DisplayName = "Participant 1",
            },
        ];
        
        await _db.AppDbContext.AddRangeAsync(Participants);

        MediaLog defaultWatchGroupDefaultMediaLog = new()
        {
            DisplayName = "Default",
            WatchGroup = testUser1DefaultWatchGroup,
            LogEntries = [
                new()
                {
                    CandidateName = "Knives Out", 
                    Attendees = [
                        new EntryAttendee()
                        {
                            Participant = Participants[0],
                        }
                    ]
                },
                new()
                {
                    CandidateName = "Jurrasic Park", 
                    Attendees = [
                        new EntryAttendee()
                        {
                            Participant = Participants[0],
                        }
                    ]
                }
            ]
        };

        var mediaLogEntityResult = await _db.AppDbContext.AddAsync(defaultWatchGroupDefaultMediaLog);

        await _db.AppDbContext.SaveChangesAsync();

        watchGroupEntityResult.State = EntityState.Detached;
        mediaLogEntityResult.State = EntityState.Detached;

        return new(
            watchGroupEntityResult.Entity,
            mediaLogEntityResult.Entity);
    }

    private record CommonTestData(WatchGroup DefaultWatchGroup, MediaLog DefaultMediaLog);
}