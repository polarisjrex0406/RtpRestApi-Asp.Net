using RtpRestApi.Models;

namespace RtpRestApi.Services
{
    public interface IExperimentService
    {
        Task<List<ExperimentResponse>> GetExperimentAsync();
    }
}
