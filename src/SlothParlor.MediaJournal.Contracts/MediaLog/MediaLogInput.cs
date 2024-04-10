using System.ComponentModel.DataAnnotations;

namespace SlothParlor.MediaJournal.Contracts.MediaLog;

public class MediaLogInput
{
    [Required]
    [MaxLength(512)]
    public string DisplayName { get; set; } = "";
}