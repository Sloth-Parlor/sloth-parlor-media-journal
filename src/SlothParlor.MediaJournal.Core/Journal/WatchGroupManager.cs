using SlothParlor.MediaJournal.Contracts.WatchGroup;
using SlothParlor.MediaJournal.Core.Application;
using SlothParlor.MediaJournal.Data;
using SlothParlor.MediaJournal.Data.Models;

namespace SlothParlor.MediaJournal.Core.Journal;

public class WatchGroupManager(IAppUserProvider _appUserProvider, MediaJournalDbContext _dbContext)
{
    public async Task<WatchGroup> CreateEmptyWatchGroup(string displayName)
    {
        WatchGroupParticipant[] participants = [
            new WatchGroupParticipant()
            {
                UserId = _appUserProvider.CurrentUserId,
                DisplayName = _appUserProvider.CurrentUser.Email,
            },
        ];

        WatchGroup newWatchGroup = new()
        {
            DisplayName = displayName,
            Participants = participants,
        };

        var result = await _dbContext.AddAsync(newWatchGroup);
        await _dbContext.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<IEnumerable<WatchGroupParticipant>> AddParticipantToWatchGroup(IEnumerable<WatchGroupParticipantInput> participants)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}