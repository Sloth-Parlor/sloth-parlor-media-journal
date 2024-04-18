using SlothParlor.MediaJournal.Contracts.WatchGroup;
using SlothParlor.MediaJournal.Core.Tests.Collections;
using SlothParlor.MediaJournal.Core.Tests.Resources;
using SlothParlor.MediaJournal.Data.Models;

namespace SlothParlor.MediaJournal.Core.Tests.Mapping;

[Collection(nameof(DefaultMapperCollection))]
public class WatchGroupParticipantMappingTests(DefaultMapperFixture _defaultMapperFixture)
{
    [Fact]
    public void ToResult()
    {
        var mapper = _defaultMapperFixture.Mapper;

        var participant = new WatchGroupParticipant
        {
            ParticipantId = 1,
            UserId = Guid.NewGuid().ToString(),
            DisplayName = "Test User"
        };

        var result = mapper.Map<WatchGroupParticipantResult>(participant);

        Assert.Equal(participant.ParticipantId, result.ParticipantId);
        Assert.Equal(participant.DisplayName, result.DisplayName);
        Assert.Equal(participant.UserId, result.UserId);
    }

    [Fact]
    public void CollectionConversion_ToReadOnlyCollection()
    {
        var mapper = _defaultMapperFixture.Mapper;

        var participant1 = new WatchGroupParticipant
        {
            ParticipantId = 1,
            UserId = Guid.NewGuid().ToString(),
            DisplayName = "Test User"
        };

        var participant2 = new WatchGroupParticipant
        {
            ParticipantId = 2,
            UserId = Guid.NewGuid().ToString(),
            DisplayName = "Test User 2"
        };

        List<WatchGroupParticipant> participants = [participant1, participant2];

        var result = mapper.Map<IReadOnlyCollection<WatchGroupParticipantResult>>(participants);
    }
}
