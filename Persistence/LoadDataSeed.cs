using DefaultProject.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace DefaultProject.Persistence
{
    public class LoadDataSeed
    {
        public static async Task InsertData(AppDbContext  context, UserManager<Usuario> userManager)
        {
            if (!userManager.Users.Any())
            {
                Usuario usuario = new Usuario
                {
                    Nombre = "Rodrigo",
                    Apellido = "Calle",
                    Email = "rodrigocallegaldos@gmail.com",
                    UserName = "rcalle",
                    Telefono = "992599599"
                };


                await userManager.CreateAsync(usuario, "Rodrigo123@RCG123@.@_@..?");
            }

            if (!context.Products.Any())
            {
                context.Products.AddRange(
                    new Product
                    {
                        Id = 102,
                        Nombre = "Polo Azul",
                        Precio = (decimal)20.40,
                        FechaCreacion = DateTime.Now
                    },
                    new Product
                    {
                        Id = 103,
                        Nombre = "Polo Negro",
                        Precio = (decimal)10.20,
                        FechaCreacion = DateTime.Now
                    }
                ); ;

            }

            await context.SaveChangesAsync();

        }
    }
}
