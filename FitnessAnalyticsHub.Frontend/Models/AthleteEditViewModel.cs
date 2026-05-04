using System.ComponentModel.DataAnnotations;

namespace FitnessAnalyticsHub.Frontend.Models;

public class AthleteEditViewModel
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "Vorname ist erforderlich.")]
    [MaxLength(50, ErrorMessage = "Vorname darf maximal 50 Zeichen lang sein.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nachname ist erforderlich.")]
    [MaxLength(50, ErrorMessage = "Nachname darf maximal 50 Zeichen lang sein.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-Mail ist erforderlich.")]
    [EmailAddress(ErrorMessage = "Bitte gib eine gültige E-Mail-Adresse ein.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Geburtsdatum ist erforderlich.")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Gewicht ist erforderlich.")]
    [Range(0.1, double.MaxValue, ErrorMessage = "Gewicht muss größer als 0 sein.")]
    public double Weight { get; set; }

    [Required(ErrorMessage = "Größe ist erforderlich.")]
    [Range(0.1, double.MaxValue, ErrorMessage = "Größe muss größer als 0 sein.")]
    public double Height { get; set; }

    public string? Error { get; set; }
    public bool Loading { get; set; } = false;
    public bool Submitting { get; set; } = false;
}
