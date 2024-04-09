using AutoMapper;

namespace SlothParlor.MediaJournal.Core.Mapping;

internal class WatchGroupProfile : Profile
{
    public WatchGroupProfile()
    {
        CreateMap<Data.Models.WatchGroup, Contracts.WatchGroup.WatchGroupResult>()
            .ConstructUsing((entity, context) => new(
                entity.WatchGroupId,
                entity.DisplayName,
                entity.Participants?
                    .Select(context.Mapper.Map<Contracts.WatchGroup.WatchGroupParticipantResult>) ?? []));

        CreateMap<Contracts.WatchGroup.WatchGroupProperties, Data.Models.WatchGroup>();
    }
}
