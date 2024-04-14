using AutoMapper;

namespace SlothParlor.MediaJournal.Core.Mapping;

public static class ProfileConfiguration
{
    public static void ConfigureProfiles(IMapperConfigurationExpression mapperConfiguration)
    {
        mapperConfiguration.AddProfile<LogEntryProfile>();
        mapperConfiguration.AddProfile<MediaLogProfile>();
        mapperConfiguration.AddProfile<WatchGroupParticipantProfile>();
        mapperConfiguration.AddProfile<WatchGroupProfile>();
    }
}
