namespace JBpunch.Domain.Entities;

public class ClockData : BaseEntity
{
    public DateTime PunchTime { get; set; }
    public Guid? IdentityUserId { get; set; }
    public Guid? GpsId { get; set; }
}
