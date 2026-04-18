using Halyr.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace Halyr.Api.DTOs;

public class CreateEnvironmentDTO
{
    [Required]
    public EnvironmentType Environment { get; set; }
    public bool Enabled { get; set; }
    [Range(0, 100)]
    public int PercentageRollout { get; set; } = 100;

}