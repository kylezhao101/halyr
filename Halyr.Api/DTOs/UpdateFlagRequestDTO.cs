using System.ComponentModel.DataAnnotations;

namespace Halyr.Api.DTOs;

public class UpdateFlagRequestDTO
{
    [MaxLength(100)]
    public string? Name { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
}