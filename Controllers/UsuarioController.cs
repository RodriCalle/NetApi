using DefaultProject.Persistence.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supermarket.API.Resources.Dtos;
using System.Threading.Tasks;

namespace DefaultProject.Controllers
{
    [AllowAnonymous]

    [Route("/api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository usuarioRepository;

        public UsuarioController(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserResponseDto>> Login([FromBody] UserLoginRequestDto request)
        {
            return await usuarioRepository.Login(request);
        }

        [HttpPost("signup")]
        public async Task<ActionResult<UserResponseDto>> SignUp([FromBody] UserSignUpRequestDto request)
        {
            return await usuarioRepository.SignUp(request);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserResponseDto>> GetUser()
        {
            return await usuarioRepository.GetUsuario();
        }
    }
}
