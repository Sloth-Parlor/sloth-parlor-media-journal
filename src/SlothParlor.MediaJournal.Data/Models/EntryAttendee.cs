using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SlothParlor.MediaJournal.Data.Models;

public class EntryAttendee
{
    [Key]
    public int EntryAttendeeId { get; set; }

    public int ParticipantId { get; set; }

    public int EntryId { get; set; }

    public virtual Entry? Entry { get; set; }

    public virtual WatchGroupParticipant? Participant { get; set; }
}
