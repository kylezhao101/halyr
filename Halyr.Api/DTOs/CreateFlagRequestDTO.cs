using System.ComponentModel.DataAnnotations;

namespace Halyr.Api.DTOs;

public class CreateFlagRequestDTO
{
    [Required]
    [MaxLength(100)]
    [RegularExpression(
        "^[a-z0-9]+(-[a-z0-9]+)*$",
        ErrorMessage = "Key must be lowercase kebab-case (e.g. new-checkout-flow)"
    )]
    public string Key { get; set; } = string.Empty;
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? Description { get; set; }
}