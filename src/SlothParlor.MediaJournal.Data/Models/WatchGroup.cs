using System.ComponentModel.DataAnnotations;

namespace SlothParlor.MediaJournal.Data.Models;

public class WatchGroup
{
    [Key]
    public int WatchGroupId { get; set; }

    [Required]
    [MaxLength(512)]
    public required string DisplayName { get; set; }

    public virtual ICollection<MediaLog>? MediaLogs { get; set; }

    public virtual ICollection<WatchGroupParticipant>? Participants { get; set; }

    public virtual ICollection<User>? Owners { get; set; }
}
