using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SlothParlor.MediaJournal.Contracts.WatchGroup;
using SlothParlor.MediaJournal.Data;
using SlothParlor.MediaJournal.Data.Models;

namespace SlothParlor.MediaJournal.Core.Journal;

public class UserJournalRepository : IUserJournalRepository
{
    private readonly IQueryable<WatchGroupParticipant> _userParticipantShadowRecords;
    private readonly IQueryable<WatchGroup> _watchGroups;
    private readonly IQueryable<MediaLog> _mediaLogs;
    private readonly IMapper _mapper;

    public UserJournalRepository(MediaJournalDbContext dbContext, IMapper mapper, string userId)
    {
        _mapper = mapper;

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

    public async Task<IEnumerable<WatchGroupResult>> GetWatchGroupsAsync()
    {
        var watchGroups = await _watchGroups.ToListAsync();

        return watchGroups.Select(_mapper.Map<WatchGroupResult>);
    }

    public async Task<IEnumerable<MediaLog>> GetMediaLogsAsync()
    {
        return await _mediaLogs.ToListAsync();
    }
}
