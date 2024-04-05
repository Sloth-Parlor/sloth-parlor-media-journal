using System.ComponentModel.DataAnnotations;

namespace SlothParlor.MediaJournal.Data.Models;

public class WatchGroupParticipant
{
    [Key]
    public int ParticipantId { get; set; }

    public int WatchGroupId { get; set; }

    public required string UserId { get; set; }

    [Required]
    [MaxLength(256)]
    public required string DisplayName { get; set; }

    public virtual WatchGroup? WatchGroup { get; set; }

    public virtual User? User { get; set; }
}
