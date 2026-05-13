using System.Diagnostics.CodeAnalysis;
using JBpunch.Domain.Entities;

namespace JBpunch.Application.Contracts;

public interface IApplicationService<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T
>
    where T : BaseEntity
{
    Task<List<T>> GetListAsync();
    Task<T?> GetAsync(Guid id);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(Guid id, T entity);
    Task DeleteAsync(Guid id);
}
