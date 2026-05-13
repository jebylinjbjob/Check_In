namespace JBpunch.DTOs;

public record CreateGpsPuncheDto
{
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public Guid? SysGpsPunche { get; init; }
}

public record UpdateGpsPuncheDto
{
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public Guid? SysGpsPunche { get; init; }
}

public record GpsPuncheResponseDto
{
    public Guid Id { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public Guid? SysGpsPunche { get; init; }
    public DateTime CreationTime { get; init; }
    public Guid? CreatorId { get; init; }
}
