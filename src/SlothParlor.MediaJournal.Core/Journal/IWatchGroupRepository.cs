using Microsoft.EntityFrameworkCore.ChangeTracking;
using SlothParlor.MediaJournal.Contracts.WatchGroup;

namespace SlothParlor.MediaJournal.Core.Journal;

public interface IWatchGroupRepository
{
    string UserId { get; }

    Task<WatchGroupResult> CreateAsync(WatchGroupInput properties);
    
    Task<WatchGroupResult> DeleteAsync(int watchGroupId);
    
    Task<IReadOnlyCollection<WatchGroupResult>> GetAsync();
    
    Task<WatchGroupResult> GetAsync(int watchGroupId);
    
    Task<WatchGroupResult> UpdateAsync(
        int watchGroupId, 
        WatchGroupInput input, 
        Func<EntityEntry<Data.Models.WatchGroup>, Task>? onUpdated = null);
}
