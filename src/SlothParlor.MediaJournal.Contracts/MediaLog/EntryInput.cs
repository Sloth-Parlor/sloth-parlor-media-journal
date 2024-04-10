
namespace SlothParlor.MediaJournal.Contracts.MediaLog;

public class EntryInput
{
    public string? CandidateName { get; set; }

    public IReadOnlySet<EntryAttendeeInput>? Attendees { get; set; }
}
