using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SlothParlor.MediaJournal.Data.Models;

public class Entry
{
    [Key]
    public int EntryId { get; set; }

    public int MediaLogId { get; set; }

    public string? CandidateName { get; set; }

    public virtual MediaLog? MediaLog { get; set; }

    public virtual IEnumerable<EntryAttendee>? Attendees { get; set; }
}
