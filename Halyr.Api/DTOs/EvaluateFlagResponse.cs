
using System.Text.Json.Serialization;

using Halyr.Api.Enums;

namespace Halyr.Api.DTOs;

public class EvaluateFlagResponse
{
    public string FlagKey { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EnvironmentType Environment { get; set; } = EnvironmentType.Development;
    public bool Enabled { get; set; }
    public int Bucket { get; set; }
    public int PercentageRollout { get; set; }
}