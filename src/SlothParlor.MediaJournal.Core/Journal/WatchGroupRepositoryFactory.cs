using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SlothParlor.MediaJournal.Data;

namespace SlothParlor.MediaJournal.Core.Journal;

public class WatchGroupRepositoryFactory(MediaJournalDbContext _dbContext, IMapper _mapper) 
    : IWatchGroupRepositoryFactory
{
    public IWatchGroupRepository Create(string userId) => 
        new WatchGroupRepository(_dbContext, _mapper)
        {
            UserId = userId
        };
}
