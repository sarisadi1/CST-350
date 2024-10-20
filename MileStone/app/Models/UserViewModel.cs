
using System.ComponentModel.DataAnnotations;


public class UserViewModel
{
    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }

    [Required]
    public required string Sex { get; set; }

    [Required]
    [Range(18, 100, ErrorMessage = "Age must be between 18 and 100.")]
    [Display(Name = "Age")]
    public required int Age { get; set; }

    [Required]
    public required string State { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public required string Password { get; set; }
}
