using System.Text.Json.Serialization;

using Halyr.Api.Enums;

namespace Halyr.Api.DTOs;

public class EvaluateFlagRequestDTO
{
    public string FlagKey { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EnvironmentType Environment { get; set; } = EnvironmentType.Development;

}