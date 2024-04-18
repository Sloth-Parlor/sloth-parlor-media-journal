namespace SlothParlor.MediaJournal.Core.Journal;

public interface IWatchGroupRepositoryFactory
{
    IWatchGroupRepository Create(string userId);
}
