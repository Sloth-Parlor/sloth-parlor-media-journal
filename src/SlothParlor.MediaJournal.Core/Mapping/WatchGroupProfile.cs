using AutoMapper;

namespace SlothParlor.MediaJournal.Core.Mapping;

public class WatchGroupProfile : Profile
{
    public WatchGroupProfile()
    {
        CreateMap<Data.Models.WatchGroup, Contracts.WatchGroup.WatchGroupResult>()
            .ConstructUsing((entity, context) =>
            {
                var participants = context.Mapper.Map<IReadOnlyCollection<Contracts.WatchGroup.WatchGroupParticipantResult>?>(entity.Participants);

                return new Contracts.WatchGroup.WatchGroupResult(entity.WatchGroupId, entity.DisplayName, participants);
            });

        CreateMap<Contracts.WatchGroup.WatchGroupInput, Data.Models.WatchGroup>()
            .ReverseMap();
    }
}
