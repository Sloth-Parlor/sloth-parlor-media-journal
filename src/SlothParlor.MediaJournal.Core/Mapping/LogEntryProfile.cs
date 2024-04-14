using AutoMapper;

namespace SlothParlor.MediaJournal.Core.Mapping;

internal class LogEntryProfile : Profile
{
    public LogEntryProfile()
    {
        CreateMap<Data.Models.Entry, Contracts.MediaLog.EntryResult>()
            .ConstructUsing((entity, context) => new(
                entity.EntryId,
                entity.MediaLogId,
                entity.CandidateName));

        CreateMap<Contracts.MediaLog.EntryInput, Data.Models.Entry>();
    }
}
