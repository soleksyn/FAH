using System.ComponentModel.DataAnnotations;

namespace SportMatrix.Frontend.Models;

public class AthleteEditViewModel
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(50, ErrorMessage = "First name must be at most 50 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(50, ErrorMessage = "Last name must be at most 50 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of birth is required.")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Weight is required.")]
    [Range(20, 300, ErrorMessage = "Weight must be between 20 and 300 kg.")]
    public double Weight { get; set; }

    [Required(ErrorMessage = "Height is required.")]
    [Range(100, 250, ErrorMessage = "Height must be between 100 and 250 cm.")]
    public double Height { get; set; }

    public string? Error { get; set; }
    public bool Loading { get; set; } = false;
    public bool Submitting { get; set; } = false;
}
