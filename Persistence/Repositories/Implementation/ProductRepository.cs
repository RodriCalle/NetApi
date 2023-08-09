using DefaultProject.Models;
using DefaultProject.Persistence.Repositories.Interfaces;
using DefaultProject.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DefaultProject.Persistence.Repositories.Implementation
{
    public class ProductRepository : IProductRepository
    {
        protected readonly AppDbContext context;
        protected readonly IUsuarioSesion usuarioSesion;
        protected readonly UserManager<Usuario> userManager;

        public ProductRepository(AppDbContext context, IUsuarioSesion usuarioSesion, UserManager<Usuario> userManager)
        {
            this.context = context;
            this.usuarioSesion = usuarioSesion;
            this.userManager = userManager;
        }

        public async Task CreateProduct(Product product)
        {
            var usuario = await userManager.FindByNameAsync(usuarioSesion.GetUserSessionId());
            product.FechaCreacion = DateTime.Now;
            product.UsuarioId = Guid.Parse(usuario.Id);
            await context.Products.AddAsync(product);
        }

        public void UpdateProduct(Product product)
        {
            context.Products.Update(product);
        }

        public void DeleteProduct(int id)
        {
            var product = context.Products.FirstOrDefault(p => p.Id == id);
            context.Products.Remove(product);
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await context.Products.ToListAsync();
        }

        public async Task<Product> GetProductById(int id)
        {
            return await context.Products.FindAsync(id);
        }
    }
}
