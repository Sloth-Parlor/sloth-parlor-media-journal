using SlothParlor.MediaJournal.Contracts.WatchGroup;
using SlothParlor.MediaJournal.Data.Models;

public interface IUserJournalRepository
{
    Task<IEnumerable<WatchGroupResult>> GetWatchGroupsAsync();
    Task<IEnumerable<MediaLog>> GetMediaLogsAsync();
}