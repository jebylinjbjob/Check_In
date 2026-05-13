using JBpunch.Domain.Entities;
using JBpunch.DTOs;
using JBpunch.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JBpunch.Services;

public interface IClockDataService
{
    Task<List<ClockDataResponseDto>> GetAllAsync(ClockDataQueryDto query);
    Task<ClockDataResponseDto?> GetByIdAsync(Guid id);
    Task<ClockDataResponseDto> CreateAsync(CreateClockDataDto dto);
    Task DeleteAsync(Guid id);
}

public class ClockDataService : IClockDataService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<ClockData> _repository;

    public ClockDataService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = unitOfWork.Repository<ClockData>();
    }

    public async Task<List<ClockDataResponseDto>> GetAllAsync(ClockDataQueryDto query)
    {
        var q = _repository.GetQueryable();

        if (query.IdentityUserId.HasValue)
            q = q.Where(e => e.IdentityUserId == query.IdentityUserId);

        if (query.From.HasValue)
            q = q.Where(e => e.PunchTime >= query.From.Value);

        if (query.To.HasValue)
            q = q.Where(e => e.PunchTime <= query.To.Value);

        var limit = Math.Clamp(query.Limit, 1, 500);

        return await q.OrderByDescending(e => e.PunchTime)
            .Take(limit)
            .Select(e => ToDto(e))
            .ToListAsync();
    }

    public async Task<ClockDataResponseDto?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : ToDto(entity);
    }

    public async Task<ClockDataResponseDto> CreateAsync(CreateClockDataDto dto)
    {
        var entity = new ClockData
        {
            PunchTime = dto.PunchTime,
            IdentityUserId = dto.IdentityUserId,
            GpsId = dto.GpsId,
        };

        await _repository.InsertAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    private static ClockDataResponseDto ToDto(ClockData e) =>
        new()
        {
            Id = e.Id,
            PunchTime = e.PunchTime,
            IdentityUserId = e.IdentityUserId,
            GpsId = e.GpsId,
            CreationTime = e.CreationTime,
            CreatorId = e.CreatorId,
        };
}
