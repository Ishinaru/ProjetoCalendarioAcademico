using CalendarioAcademico.Data.DBContext.Repositories.Generic;
namespace CalendarioAcademico.Data.DBContext.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        //IRepositories
        TRepo? GetRepository<TRepo, TEntity>() where TRepo : class where TEntity : class;
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
