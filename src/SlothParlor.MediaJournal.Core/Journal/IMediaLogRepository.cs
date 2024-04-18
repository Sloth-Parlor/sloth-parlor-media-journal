using Microsoft.EntityFrameworkCore.ChangeTracking;
using SlothParlor.MediaJournal.Contracts.MediaLog;

namespace SlothParlor.MediaJournal.Core.Journal;

public interface IMediaLogRepository
{
    int WatchGroupId { get; }

    Task<MediaLogResult> CreateEmptyAsync(MediaLogInput properties);

    Task<MediaLogResult> DeleteAsync(int mediaLogId);

    Task<IReadOnlyCollection<MediaLogResult>> GetAsync();

    Task<MediaLogResult> GetAsync(int mediaLogId);

    Task<MediaLogResult> UpdateAsync(
        int mediaLogId,
        MediaLogInput properties,
        Func<EntityEntry<Data.Models.MediaLog>, Task>? changeTracking = null);
}
