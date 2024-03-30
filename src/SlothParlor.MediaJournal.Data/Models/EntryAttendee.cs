using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SlothParlor.MediaJournal.Data.Models;

public class EntryAttendee
{
    [Key]
    public int EntryAttendeeId { get; set; }

    public int EntryId { get; set; }

    public required Entry Entry { get; set; }

    public required WatchGroupParticipant Participant { get; set; }
}
