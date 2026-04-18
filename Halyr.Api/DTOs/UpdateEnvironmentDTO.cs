using System.ComponentModel.DataAnnotations;

namespace Halyr.Api.DTOs;

public class UpdateEnvironmentDTO
{
    public bool? Enabled { get; set; }
    [Range(0, 100)]
    public int? PercentageRollout { get; set; }
}