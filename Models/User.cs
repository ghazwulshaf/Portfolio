using System.ComponentModel.DataAnnotations;

namespace GhazwulShaf.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Username { get; set; } = default!;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = default!;

    public string Role { get; set; } = default!;
}