
using DefaultProject.Models;

namespace DefaultProject.Security
{
    public interface IJwtGenerator
    {
        string GenerateToken(Usuario usuario);
    }
}
