using SlothParlor.MediaJournal.Core.Application;
using SlothParlor.MediaJournal.Data;
using SlothParlor.MediaJournal.Data.Models;

namespace SlothParlor.MediaJournal.Core.Journal;

public class JournalManager(MediaJournalDbContext _dbContext)
{
    public async Task<MediaLog> CreateNewMediaLog(int watchGroupId, string displayName)
    {
        MediaLog newMediaLog = new()
        {
            WatchGroupId = watchGroupId,
            DisplayName = displayName, 
        };

        var result = await _dbContext.AddAsync(newMediaLog);
        await _dbContext.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<Entry> AddMediaLogEntry(int mediaLogId, string candidateName)
    {
        Entry newEntry = new()
        {
            MediaLogId = mediaLogId,
            CandidateName = candidateName,

        };

        var result = await _dbContext.AddAsync(newEntry);
        await _dbContext.SaveChangesAsync();

        return result.Entity;
    }
}