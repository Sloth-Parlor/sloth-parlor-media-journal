using AutoMapper;
using SlothParlor.MediaJournal.Core.Mapping;

namespace SlothParlor.MediaJournal.Core.DataAccessTests.Resources;

public class ApplicationMapperFixture
{
    private readonly IMapper _mapper;

    public ApplicationMapperFixture()
    {
        var mapperConfiguration = new MapperConfiguration(ProfileConfiguration.ConfigureProfiles);

        _mapper = mapperConfiguration.CreateMapper();
    }

    public IMapper Mapper => _mapper;
}
