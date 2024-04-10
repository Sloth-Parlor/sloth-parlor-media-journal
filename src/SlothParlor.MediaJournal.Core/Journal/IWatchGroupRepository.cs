namespace SlothParlor.MediaJournal.Core.Journal;

public interface IWatchGroupRepository
{
    Task CreateEmptyAsync();
    Task DeleteAsync(int watchGroupId);
    Task FindAsync(int watchGroupId);
    Task UpdateAsync(int watchGroupId);
}
