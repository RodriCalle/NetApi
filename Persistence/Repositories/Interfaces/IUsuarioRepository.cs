using Supermarket.API.Resources.Dtos;
using System.Threading.Tasks;

namespace DefaultProject.Persistence.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<UserResponseDto> GetUsuario();
        Task<UserResponseDto> Login(UserLoginRequestDto request);
        Task<UserResponseDto> SignUp(UserSignUpRequestDto request);
    }
}
