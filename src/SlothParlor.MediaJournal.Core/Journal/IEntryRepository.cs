namespace SlothParlor.MediaJournal.Core.Journal;

public interface IEntryRepository
{
    int MediaLogId { get; }

    Task<Contracts.MediaLog.EntryResult> CreateAsync(Contracts.MediaLog.EntryInput properties);

    Task DeleteAsync(int entryId);

    Task<IReadOnlyCollection<Contracts.MediaLog.EntryResult>> GetAsync();

    Task<Contracts.MediaLog.EntryResult> GetAsync(int entryId);

    Task<Contracts.MediaLog.EntryResult> UpdateAsync(int entryId, Contracts.MediaLog.EntryInput properties);
}
