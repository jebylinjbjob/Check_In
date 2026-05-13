using System.Diagnostics.CodeAnalysis;
using JBpunch.Application.Contracts;
using JBpunch.Domain.Entities;
using JBpunch.Repositories;

namespace JBpunch.Application.Services;

public class ApplicationService<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>
    : IApplicationService<T>
    where T : BaseEntity
{
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly IRepository<T> _repository;

    public ApplicationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = unitOfWork.Repository<T>();
    }

    public virtual async Task<List<T>> GetListAsync()
    {
        return await _repository.GetAllAsync();
    }

    public virtual async Task<T?> GetAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        await _repository.InsertAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> UpdateAsync(Guid id, T entity)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            throw new System.Collections.Generic.KeyNotFoundException(
                $"Entity with id {id} not found"
            );

        entity.Id = id;
        await _repository.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity;
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
