using System.ComponentModel.DataAnnotations;

namespace SlothParlor.MediaJournal.Data.Models;

public class MediaLog
{
    [Key]
    public int MediaLogId { get; set; }

    public int WatchGroupId { get; set; }

    [Required]
    [MaxLength(512)]
    public required string DisplayName { get; set; }

    public virtual IEnumerable<Entry>? LogEntries { get; set; }

    public virtual WatchGroup? WatchGroup { get; set; }
}
