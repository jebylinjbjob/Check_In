using JBpunch.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JBpunch.Infrastructure;

public class JBpunchDbContext(DbContextOptions<JBpunchDbContext> options) : DbContext(options)
{
    public DbSet<ClockData> ClockDatas => Set<ClockData>();
    public DbSet<GpsPunche> GpsPunches => Set<GpsPunche>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ClockData>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<GpsPunche>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }
}
