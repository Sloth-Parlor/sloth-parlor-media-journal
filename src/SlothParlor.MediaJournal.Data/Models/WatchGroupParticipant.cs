using System.ComponentModel.DataAnnotations;

namespace SlothParlor.MediaJournal.Data.Models;

public class WatchGroupParticipant
{
    [Key]
    public int ParticipantId { get; set; }

    public int UserId { get; set; }

    public int WatchGroupId { get; set; }

    [Required]
    [MaxLength(256)]
    public required string DisplayName { get; set; }

    public required WatchGroup WatchGroup { get; set; }

    public required User User { get; set; }
}
