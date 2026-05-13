namespace JBpunch.Domain.Entities;

public class GpsPunche : BaseEntity
{
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Guid? SysGpsPunche { get; set; }
}
