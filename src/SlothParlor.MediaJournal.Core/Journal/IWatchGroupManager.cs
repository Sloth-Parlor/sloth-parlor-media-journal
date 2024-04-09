using SlothParlor.MediaJournal.Contracts.WatchGroup;

namespace SlothParlor.MediaJournal.Core.Journal;

public interface IWatchGroupManager
{
    Task<WatchGroupResult> CreateEmptyWatchGroupAsync(WatchGroupProperties properties);
}
