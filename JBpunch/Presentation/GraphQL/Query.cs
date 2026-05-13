using JBpunch.DTOs;
using JBpunch.Services;

namespace JBpunch.Presentation.GraphQL;

public class Query
{
    public Task<List<ClockDataResponseDto>> GetClockDatas(
        [Service] IClockDataService service,
        ClockDataQueryDto? query = null
    ) => service.GetAllAsync(query ?? new ClockDataQueryDto());

    public Task<ClockDataResponseDto?> GetClockData([Service] IClockDataService service, Guid id) =>
        service.GetByIdAsync(id);

    public Task<List<GpsPuncheResponseDto>> GetGpsPunches([Service] IGpsPuncheService service) =>
        service.GetAllAsync();

    public Task<GpsPuncheResponseDto?> GetGpsPunche([Service] IGpsPuncheService service, Guid id) =>
        service.GetByIdAsync(id);
}
