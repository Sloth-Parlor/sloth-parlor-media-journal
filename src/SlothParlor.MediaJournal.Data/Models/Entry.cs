using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SlothParlor.MediaJournal.Data.Models;

public class Entry
{
    [Key]
    public int MediaLogEntryId { get; set; }

    public int MediaLogId { get; set; }

    public string? CandidateName { get; set; }

    public required MediaLog MediaLog { get; set; }

    public required IEnumerable<EntryAttendee> Attendees { get; set; }
}
