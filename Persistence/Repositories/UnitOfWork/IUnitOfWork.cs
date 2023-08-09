using DefaultProject.Persistence.Repositories.Interfaces;

namespace DefaultProject.Persistence.Repositories.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
        IProductRepository ProductRepository { get; }
    }
}
