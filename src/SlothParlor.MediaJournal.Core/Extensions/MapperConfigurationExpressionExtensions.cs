using AutoMapper;
using SlothParlor.MediaJournal.Core.Mapping;

namespace SlothParlor.MediaJournal.Core.Extensions;

public static class IMapperConfigurationExpressionExtensions
{
    public static IMapperConfigurationExpression AddDefaultProfiles(this IMapperConfigurationExpression cfg)
    {
        cfg.AddProfile<LogEntryProfile>();
        cfg.AddProfile<MediaLogProfile>();
        cfg.AddProfile<WatchGroupParticipantProfile>();
        cfg.AddProfile<WatchGroupProfile>();

        return cfg;
    }
}
