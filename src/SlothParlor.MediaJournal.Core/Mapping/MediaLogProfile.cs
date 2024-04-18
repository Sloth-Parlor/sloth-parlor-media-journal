using AutoMapper;

namespace SlothParlor.MediaJournal.Core.Mapping;

public class MediaLogProfile : Profile
{
    public MediaLogProfile()
    {
        CreateMap<Data.Models.MediaLog, Contracts.MediaLog.MediaLogResult>()
            .ConstructUsing((entity, context) => new(
                entity.MediaLogId,
                entity.DisplayName,
                entity.LogEntries?
                    .Select(context.Mapper.Map<Contracts.MediaLog.EntryResult>) ?? []));

        CreateMap<Contracts.MediaLog.MediaLogInput, Data.Models.MediaLog>();
    }
}
