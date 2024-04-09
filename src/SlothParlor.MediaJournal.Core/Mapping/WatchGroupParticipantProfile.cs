using AutoMapper;

namespace SlothParlor.MediaJournal.Core.Mapping;

internal class WatchGroupParticipantProfile : Profile
{
    public WatchGroupParticipantProfile()
    {
        CreateMap<Data.Models.WatchGroupParticipant, Contracts.WatchGroup.WatchGroupParticipantResult>()
            .ConstructUsing((entity) => new(entity.ParticipantId, entity.UserId, entity.DisplayName));

        CreateMap<Contracts.WatchGroup.WatchGroupParticipantInput, Data.Models.WatchGroupParticipant>();
    }
}
