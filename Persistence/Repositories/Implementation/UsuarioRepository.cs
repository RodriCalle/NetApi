using AutoMapper;
using DefaultProject.Models;
using DefaultProject.Persistence.Repositories.Interfaces;
using DefaultProject.Security;
using DefaultProject.Security.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Supermarket.API.Resources.Dtos;

namespace DefaultProject.Persistence.Repositories.Implementation
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;
        private readonly IJwtGenerator jwtGenerator;
        private readonly AppDbContext context;
        private readonly IUsuarioSesion usuarioSesion;
        private readonly IMapper mapper;

        public UsuarioRepository(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, IJwtGenerator jwtGenerator, AppDbContext context, IUsuarioSesion usuarioSesion)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.jwtGenerator = jwtGenerator;
            this.context = context;
            this.usuarioSesion = usuarioSesion;
        }

        public async Task<UserResponseDto> GetUsuario()
        {
            var usuario = await userManager.FindByNameAsync(usuarioSesion.GetUserSessionId());

            if (usuario == null)
            {
                throw new MiddlewareException(
                    System.Net.HttpStatusCode.Unauthorized,
                    new { mensaje = "El usuario del token no existe." }
                    );
            }

            var usuarioDto = MapUserToUserDto(usuario);
            return usuarioDto;
        }

        private UserResponseDto MapUserToUserDto(Usuario usuario)
        {
            return new UserResponseDto
            {
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Telefono = usuario.Telefono,
                UserName = usuario.UserName,
                Token = jwtGenerator.GenerateToken(usuario)
            };
        }

        public async Task<UserResponseDto> Login(UserLoginRequestDto request)
        {
            var usuario = await userManager.FindByEmailAsync(request.Email);

            if (usuario == null)
            {
                throw new MiddlewareException(
                    System.Net.HttpStatusCode.Unauthorized,
                    new { mensaje = "El email del usuario no existe." }
                    );
            }

            var result = await signInManager.CheckPasswordSignInAsync(usuario, request.Password, false);

            if (result.Succeeded)
                return MapUserToUserDto(usuario);
            else
            {
                throw new MiddlewareException(
                    System.Net.HttpStatusCode.Unauthorized,
                    new { mensaje = "Las credenciales son incorrectas" }
                    );
            }

        }

        public async Task<UserResponseDto> SignUp(UserSignUpRequestDto request)
        {
            var existeEmail = await context.Users.Where(x => x.Email == request.Email).AnyAsync();
            if (existeEmail)
            {
                throw new MiddlewareException(
                    System.Net.HttpStatusCode.BadRequest,
                    new { mensaje = "El email ya se encuentra registrado" }
                    );
            }

            var existeUserName = await context.Users.Where(x => x.UserName == request.Username).AnyAsync();
            if (existeUserName)
            {
                throw new MiddlewareException(
                    System.Net.HttpStatusCode.BadRequest,
                    new { mensaje = "El username ya se encuentra registrado" }
                    );
            }

            var usuario = new Usuario
            {
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Telefono = request.Telefono,
                Email = request.Email,
                UserName = request.Username
            };

            var result = await userManager.CreateAsync(usuario, request.Password);

            if (result.Succeeded)
                return MapUserToUserDto(usuario);
            else
                throw new Exception("Error al registar al usuario.");
        }
    }
}
