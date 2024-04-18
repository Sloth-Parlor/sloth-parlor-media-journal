using AutoMapper;
using SlothParlor.MediaJournal.Core.Extensions;

namespace SlothParlor.MediaJournal.Core.DataAccessTests.Resources;

public class DefaultMapperFixture
{
    private readonly IMapper _mapper;

    public DefaultMapperFixture()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddDefaultProfiles();
        }).CreateMapper();
    }

    public IMapper Mapper => _mapper;
}
