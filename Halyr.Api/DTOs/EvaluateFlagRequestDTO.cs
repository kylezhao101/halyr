using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Halyr.Api.Enums;

namespace Halyr.Api.DTOs;

public class EvaluateFlagRequestDTO
{
    [Required]
    public string FlagKey { get; set; } = string.Empty;
    [Required]
    public Guid UserId { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Required]
    public EnvironmentType Environment { get; set; } = EnvironmentType.Development;

}