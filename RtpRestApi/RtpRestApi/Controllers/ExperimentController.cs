using Microsoft.AspNetCore.Mvc;
using RtpRestApi.Services;

namespace RtpRestApi.Controllers
{
    [ApiController]
    [Route("api/experiment")]
    public class ExperimentController(IExperimentService experimentService) : ControllerBase
    {

        [HttpGet]
        [Route("listAll")]
        public async Task<IActionResult> Get()
        {
            var response = await experimentService.GetExperimentAsync();
            return Ok(response);
        }
    }
}
