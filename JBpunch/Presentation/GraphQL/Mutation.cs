using JBpunch.DTOs;
using JBpunch.Services;

namespace JBpunch.Presentation.GraphQL;

public class Mutation
{
    public Task<ClockDataResponseDto> CreateClockData(
        [Service] IClockDataService service,
        CreateClockDataDto input
    ) => service.CreateAsync(input);

    public async Task<bool> DeleteClockData([Service] IClockDataService service, Guid id)
    {
        await service.DeleteAsync(id);
        return true;
    }

    public Task<GpsPuncheResponseDto> CreateGpsPunche(
        [Service] IGpsPuncheService service,
        CreateGpsPuncheDto input
    ) => service.CreateAsync(input);

    public Task<GpsPuncheResponseDto> UpdateGpsPunche(
        [Service] IGpsPuncheService service,
        Guid id,
        UpdateGpsPuncheDto input
    ) => service.UpdateAsync(id, input);

    public async Task<bool> DeleteGpsPunche([Service] IGpsPuncheService service, Guid id)
    {
        await service.DeleteAsync(id);
        return true;
    }
}
