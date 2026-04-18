using Halyr.Api.Enums;

namespace Halyr.Api.DTOs;

public class EnvironmentConfigResponseDTO
{
    public EnvironmentType Environment { get; set; }
    public bool Enabled { get; set; }
    public int PercentageRollout { get; set; }
}