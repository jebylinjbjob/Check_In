using System.ComponentModel.DataAnnotations;

namespace JBpunch.DTOs;

public record ClockDataQueryDto
{
    public Guid? IdentityUserId { get; init; }
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }
    public int Limit { get; init; } = 100;
}

public record CreateClockDataDto
{
    [Required]
    public DateTime PunchTime { get; init; }

    public Guid? IdentityUserId { get; init; }
    public Guid? GpsId { get; init; }
}

public record ClockDataResponseDto
{
    public Guid Id { get; init; }
    public DateTime PunchTime { get; init; }
    public Guid? IdentityUserId { get; init; }
    public Guid? GpsId { get; init; }
    public DateTime CreationTime { get; init; }
    public Guid? CreatorId { get; init; }
}
