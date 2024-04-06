using Microsoft.EntityFrameworkCore;
using SlothParlor.MediaJournal.Data;
using SlothParlor.MediaJournal.Data.Models;

namespace SlothParlor.MediaJournal.Core.Journal;

public class UserJournalRepository : IUserJournalRepository
{
    private readonly IQueryable<WatchGroupParticipant> _userParticipantShadowRecords;
    private readonly IQueryable<WatchGroup> _watchGroups;
    private readonly IQueryable<MediaLog> _mediaLogs;
    
    public UserJournalRepository(MediaJournalDbContext dbContext, string userId)
    {
        _userParticipantShadowRecords = dbContext.WatchGroupParticipants
            .Where(wgp => wgp.UserId == userId);

        _watchGroups = dbContext.WatchGroups
            .Include(wg => wg.MediaLogs)
            .Include(wg => wg.Participants)
            .Where(wg => wg.Participants!
                .Any(p => _userParticipantShadowRecords
                    .Select(p => p.ParticipantId)
                    .Contains(p.ParticipantId)));

        _mediaLogs = _watchGroups.SelectMany(wg => wg.MediaLogs!);
    }

    public async Task<IEnumerable<WatchGroup>> GetWatchGroupsAsync()
    {
        return await _watchGroups.ToListAsync();
    }

    public async Task<IEnumerable<MediaLog>> GetMediaLogsAsync()
    {
        return await _mediaLogs.ToListAsync();
    }
}
