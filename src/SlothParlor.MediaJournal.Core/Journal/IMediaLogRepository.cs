using SlothParlor.MediaJournal.Contracts.MediaLog;

namespace SlothParlor.MediaJournal.Core.Journal;

public interface IMediaLogRepository
{
    int WatchGroupId { get; }

    Task<MediaLogResult> CreateEmptyAsync(MediaLogInput properties);

    Task<MediaLogResult> DeleteAsync(int mediaLogId);

    Task<IReadOnlyCollection<MediaLogResult>> Get();

    Task<MediaLogResult> Get(int mediaLogId);

    Task<MediaLogResult> UpdateAsync(int mediaLogId, MediaLogInput properties);
}
