using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RtpRestApi.Services;

namespace RtpRestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserRepository userRepository;

        public UserController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            return Ok(userRepository.GetAllUsers());
        }
    }
}
