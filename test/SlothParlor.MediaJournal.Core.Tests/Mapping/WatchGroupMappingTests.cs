using SlothParlor.MediaJournal.Contracts.WatchGroup;
using SlothParlor.MediaJournal.Core.Tests.Collections;
using SlothParlor.MediaJournal.Core.Tests.Resources;
using SlothParlor.MediaJournal.Data.Models;

namespace SlothParlor.MediaJournal.Core.Tests.Mapping;

[Collection(nameof(DefaultMapperCollection))]
public class WatchGroupMappingTests(DefaultMapperFixture _defaultMapperFixture)
{
    [Fact]
    public void WatchGroupModel_ToWatchGroupResult_WithoutParticipants()
    {
        var mapper = _defaultMapperFixture.Mapper;

        var watchGroup = new WatchGroup
        {
            WatchGroupId = 1,
            DisplayName = "Test Watch Group",
        };

        var result = mapper.Map<WatchGroupResult>(watchGroup);

        Assert.Equal(watchGroup.WatchGroupId, result.WatchGroupId);
        Assert.Equal(watchGroup.DisplayName, result.DisplayName);
        Assert.Null(watchGroup.Participants);
    }

    [Fact]
    public void WatchGroupModel_ToWatchGroupResult_WithParticipant()
    {
        var mapper = _defaultMapperFixture.Mapper;

        var participant = new WatchGroupParticipant
        {
            ParticipantId = 1,
            UserId = Guid.NewGuid().ToString(),
            DisplayName = "Test User"
        };

        var watchGroup = new WatchGroup
        {
            WatchGroupId = 1,
            DisplayName = "Test Watch Group",
            Participants = [participant],
        };

        var result = mapper.Map<WatchGroupResult>(watchGroup);

        Assert.Equal(watchGroup.WatchGroupId, result.WatchGroupId);
        Assert.Equal(watchGroup.DisplayName, result.DisplayName);
        Assert.NotNull(watchGroup.Participants);
        Assert.Same(participant, watchGroup.Participants.Single());
    }

    [Fact]
    public void WatchGroupModel_ToWatchGroupInput()
    {
        var mapper = _defaultMapperFixture.Mapper;

        var participant = new WatchGroupParticipant
        {
            ParticipantId = 1,
            UserId = Guid.NewGuid().ToString(),
            DisplayName = "Test User"
        };

        var watchGroup = new WatchGroup
        {
            WatchGroupId = 1,
            DisplayName = "Test Watch Group",
            Participants = [participant],
        };

        var result = mapper.Map<WatchGroupInput>(watchGroup);

        Assert.Equal(watchGroup.DisplayName, result.DisplayName);
        Assert.NotNull(watchGroup.Participants);
        Assert.Same(participant, watchGroup.Participants.Single());
    }

    [Fact]
    public void WatchGroupInput_ToWatchGroupModel()
    {
        var mapper = _defaultMapperFixture.Mapper;

        var watchGroup = new WatchGroupInput
        {
            DisplayName = "Test Watch Group",
        };

        var result = mapper.Map<WatchGroup>(watchGroup);

        Assert.Equal(watchGroup.DisplayName, result.DisplayName);
    }
}
