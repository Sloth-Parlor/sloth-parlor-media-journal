using System.ComponentModel.DataAnnotations;

namespace SlothParlor.MediaJournal.Contracts.WatchGroup;

public class WatchGroupInput
{
    [Required]
    [MaxLength(512)]
    public string DisplayName { get; set; } = "";
}
