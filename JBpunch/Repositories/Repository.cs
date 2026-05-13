using System.Diagnostics.CodeAnalysis;
using JBpunch.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JBpunch.Repositories;

public interface IRepository<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>
    where T : BaseEntity
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task<T> InsertAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    IQueryable<T> GetQueryable();
}

public class Repository<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>
    : IRepository<T>
    where T : BaseEntity
{
    protected readonly MyDbContext _db;

    public Repository(MyDbContext db)
    {
        _db = db;
    }

    public virtual IQueryable<T> GetQueryable()
    {
        return _db.Set<T>();
    }

    public virtual async Task<List<T>> GetAllAsync()
    {
        return await _db.Set<T>().ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _db.Set<T>().FindAsync(id);
    }

    public virtual Task<T> InsertAsync(T entity)
    {
        _db.Set<T>().Add(entity);
        return Task.FromResult(entity);
    }

    public virtual Task<T> UpdateAsync(T entity)
    {
        _db.Set<T>().Update(entity);
        return Task.FromResult(entity);
    }

    public virtual Task DeleteAsync(Guid id)
    {
        var entity = _db.Set<T>().Find(id);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.DeletionTime = DateTime.UtcNow;
        }
        return Task.CompletedTask;
    }
}
