using System.ComponentModel.DataAnnotations;

namespace CustomerHub.Models;

public class Customer
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string? Phone { get; set; }

    public string? City { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Joined on")]
    public DateOnly JoinedOn { get; set; } = DateOnly.FromDateTime(DateTime.Today);
}