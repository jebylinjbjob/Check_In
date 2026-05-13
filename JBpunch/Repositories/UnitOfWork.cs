using System.Diagnostics.CodeAnalysis;
using JBpunch.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace JBpunch.Repositories;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>()
        where T : BaseEntity;
    Task<int> SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly MyDbContext _db;
    private IDbContextTransaction? _transaction;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(MyDbContext db)
    {
        _db = db;
    }

    public IRepository<T> Repository<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T
    >()
        where T : BaseEntity
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new Repository<T>(_db);
        }
        return (IRepository<T>)_repositories[type];
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _db.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        _transaction = await _db.Database.BeginTransactionAsync();
        return _transaction;
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
    }
}
