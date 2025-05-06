using CalendarioAcademico.Domain.DTO.Pagination;
using System.Linq.Expressions;

namespace CalendarioAcademico.Data.DBContext.Repositories.Generic
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        TEntity BuscarPorId(int id);
        Task<TEntity> BuscarPorIdAsync(int id);
        IEnumerable<TEntity> Listar();
        Task<IEnumerable<TEntity>> Listar_Por_Condicao(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes);
        Task<IEnumerable<TEntity>> ListarAsync(params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity> CadastrarAsync(TEntity entity);
        Task<bool> Atualizar(TEntity entity);
        Task<TEntity> DesativarAsync(int id);
        Task<PaginacaoDTO<TEntity>> Listar_Por_Paginacao(Expression<Func<TEntity, bool>> filtro = null, 
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string incluirPropriedades = " ",
            int tamanhoPagina = 10,
            int numeroPagina = 1);
    }
}
