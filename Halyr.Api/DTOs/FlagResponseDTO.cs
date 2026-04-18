namespace Halyr.Api.DTOs;

public class FlagResponseDTO
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = null;

    public List<EnvironmentConfigResponseDTO> Environments { get; set; } = [];
}