using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SlothParlor.MediaJournal.Data.Models;

[Table(nameof(User), Schema = Constants.Schema.Users)]
public class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    public required string Provider { get; set; }

    [Required]
    public required string ObjectId { get; set; }

    [Required]
    public required string Email { get; set; }
}
