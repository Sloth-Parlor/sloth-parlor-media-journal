using SlothParlor.MediaJournal.Data.Models;

public interface IUserJournalRepository
{
    Task<IEnumerable<WatchGroup>> GetWatchGroupsAsync();
    Task<IEnumerable<MediaLog>> GetMediaLogsAsync();
}