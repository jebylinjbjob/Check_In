using System.Security.Claims;
using JBpunch.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JBpunch;

public class MyDbContext : DbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options) { }

    public MyDbContext(
        DbContextOptions<MyDbContext> options,
        IHttpContextAccessor httpContextAccessor
    )
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<ClockData> ClockDatas { get; set; }
    public DbSet<GpsPunche> GpsPunches { get; set; }

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

    public override int SaveChanges()
    {
        ApplyAuditInfo();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditInfo()
    {
        var now = DateTime.UtcNow;
        var user = _httpContextAccessor?.HttpContext?.User;
        Guid? userId = null;
        if (user?.Identity?.IsAuthenticated == true)
        {
            var sub = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
            if (Guid.TryParse(sub, out var parsedId))
                userId = parsedId;
        }

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreationTime = now;
                    entry.Entity.CreatorId = userId;
                    break;
                case EntityState.Modified when entry.Entity.IsDeleted:
                    entry.Entity.DeletionTime ??= now;
                    entry.Entity.DeleterId = userId;
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModificationTime = now;
                    entry.Entity.LastModifierId = userId;
                    break;
            }
        }
    }
}
