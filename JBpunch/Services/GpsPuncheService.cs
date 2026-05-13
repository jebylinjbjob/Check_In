using JBpunch.Domain.Entities;
using JBpunch.DTOs;
using JBpunch.Repositories;

namespace JBpunch.Services;

public interface IGpsPuncheService
{
    Task<List<GpsPuncheResponseDto>> GetAllAsync();
    Task<GpsPuncheResponseDto?> GetByIdAsync(Guid id);
    Task<GpsPuncheResponseDto> CreateAsync(CreateGpsPuncheDto dto);
    Task<GpsPuncheResponseDto> UpdateAsync(Guid id, UpdateGpsPuncheDto dto);
    Task DeleteAsync(Guid id);
}

public class GpsPuncheService : IGpsPuncheService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<GpsPunche> _repository;

    public GpsPuncheService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = unitOfWork.Repository<GpsPunche>();
    }

    public async Task<List<GpsPuncheResponseDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(ToDto).ToList();
    }

    public async Task<GpsPuncheResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : ToDto(entity);
    }

    public async Task<GpsPuncheResponseDto> CreateAsync(CreateGpsPuncheDto dto)
    {
        var entity = new GpsPunche
        {
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            SysGpsPunche = dto.SysGpsPunche,
        };

        await _repository.InsertAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task<GpsPuncheResponseDto> UpdateAsync(Guid id, UpdateGpsPuncheDto dto)
    {
        var entity =
            await _repository.GetByIdAsync(id)
            ?? throw new System.Collections.Generic.KeyNotFoundException(
                $"GpsPunche with id {id} not found"
            );

        entity.Latitude = dto.Latitude;
        entity.Longitude = dto.Longitude;
        entity.SysGpsPunche = dto.SysGpsPunche;

        await _repository.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    private static GpsPuncheResponseDto ToDto(GpsPunche e) =>
        new()
        {
            Id = e.Id,
            Latitude = e.Latitude,
            Longitude = e.Longitude,
            SysGpsPunche = e.SysGpsPunche,
            CreationTime = e.CreationTime,
            CreatorId = e.CreatorId,
        };
}
