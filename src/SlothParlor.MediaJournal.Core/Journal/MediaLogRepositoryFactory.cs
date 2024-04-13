using AutoMapper;
using SlothParlor.MediaJournal.Data;

namespace SlothParlor.MediaJournal.Core.Journal;

public class MediaLogRepositoryFactory(MediaJournalDbContext _dbContext, IMapper _mapper) : IMediaLogRepositoryFactory
{
    public IMediaLogRepository Create(int watchGroupId)
    {
        return new MediaLogRepository(_dbContext, _mapper)
        {
            WatchGroupId = watchGroupId,
        };
    }
}
