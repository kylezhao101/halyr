using Halyr.Api.Enums;

namespace Halyr.Api.DTOs;

public class FlagEnvironmentDTO
{
    public EnvironmentType Environment { get; set; }
    public bool Enabled { get; set; }
    public int PercentageRollout { get; set; }
}