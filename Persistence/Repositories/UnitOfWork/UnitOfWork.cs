using DefaultProject.Models;
using DefaultProject.Persistence.Repositories.Implementation;
using DefaultProject.Persistence.Repositories.Interfaces;
using DefaultProject.Security;
using Microsoft.AspNetCore.Identity;

namespace DefaultProject.Persistence.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly AppDbContext context;
        protected readonly IUsuarioSesion usuarioSesion;
        protected readonly UserManager<Usuario> userManager;
        public IProductRepository ProductRepository { get; private set; }

        public UnitOfWork(
            AppDbContext context,
            IUsuarioSesion usuarioSesion,
            UserManager<Usuario> userManager)
        {
            this.context = context;
            this.usuarioSesion = usuarioSesion;
            this.userManager = userManager;
            ProductRepository = new ProductRepository(context, usuarioSesion, userManager);
        }

        public async Task CompleteAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
