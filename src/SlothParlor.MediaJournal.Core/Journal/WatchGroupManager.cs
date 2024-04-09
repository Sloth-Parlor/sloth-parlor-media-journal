using AutoMapper;
using SlothParlor.MediaJournal.Contracts.WatchGroup;
using SlothParlor.MediaJournal.Core.Application;
using SlothParlor.MediaJournal.Data;
using SlothParlor.MediaJournal.Data.Models;

namespace SlothParlor.MediaJournal.Core.Journal;

public class WatchGroupManager(IAppUserProvider _appUserProvider, MediaJournalDbContext _dbContext, IMapper _mapper)
    : IWatchGroupManager
{
    public async Task<WatchGroupResult> CreateEmptyWatchGroupAsync(WatchGroupProperties properties)
    {
        var newWatchGroup = _mapper.Map<WatchGroup>(properties);

        newWatchGroup.Participants = [
            new WatchGroupParticipant()
            {
                UserId = _appUserProvider.CurrentUserId,
                DisplayName = _appUserProvider.CurrentUser.Email,
            },
        ];

        var result = await _dbContext.AddAsync(newWatchGroup);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<WatchGroupResult>(result.Entity);
    }

    public async Task<IEnumerable<WatchGroupParticipant>> AddParticipantToWatchGroup(IEnumerable<WatchGroupParticipantInput> participants)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}