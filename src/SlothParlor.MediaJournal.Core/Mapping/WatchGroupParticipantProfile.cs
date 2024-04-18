using AutoMapper;
using SlothParlor.MediaJournal.Data.Models;

namespace SlothParlor.MediaJournal.Core.Mapping;

public class WatchGroupParticipantProfile : Profile
{
    public WatchGroupParticipantProfile()
    {
        CreateMap<ICollection<WatchGroupParticipant>?, IReadOnlyCollection<Contracts.WatchGroup.WatchGroupParticipantResult>?>()
            .ConvertUsing((source, _, context) =>
            {
                return source?
                    .Select(context.Mapper.Map<Contracts.WatchGroup.WatchGroupParticipantResult>)
                    .ToList().AsReadOnly();
            });

        CreateMap<Data.Models.WatchGroupParticipant, Contracts.WatchGroup.WatchGroupParticipantResult>()
            .ForCtorParam(
                nameof(Contracts.WatchGroup.WatchGroupParticipantResult.ParticipantId),
                (config) => config.MapFrom(wgp => wgp.ParticipantId))
            .ForCtorParam(
                nameof(Contracts.WatchGroup.WatchGroupParticipantResult.UserId),
                (config) => config.MapFrom(wgp => wgp.UserId))
            .ForCtorParam(
                nameof(Contracts.WatchGroup.WatchGroupParticipantResult.DisplayName),
                (config) => config.MapFrom(wgp => wgp.DisplayName));

        CreateMap<Contracts.WatchGroup.WatchGroupParticipantInput, Data.Models.WatchGroupParticipant>();
    }
}
