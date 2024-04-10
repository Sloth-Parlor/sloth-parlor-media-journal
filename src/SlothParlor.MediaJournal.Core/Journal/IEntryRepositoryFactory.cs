namespace SlothParlor.MediaJournal.Core.Journal;

public interface IEntryRepositoryFactory
{
    IEntryRepository Create(string userId, int mediaLogId);
}
