
namespace SlothParlor.MediaJournal.Contracts.MediaLog;

public record MediaLogResult(int MediaLogId, string DisplayName, IEnumerable<EntryResult> LogEntries);
