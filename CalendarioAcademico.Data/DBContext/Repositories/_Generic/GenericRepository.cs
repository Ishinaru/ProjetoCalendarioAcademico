using CalendarioAcademico.Domain.DTO.Pagination;
using CalendarioAcademico.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace CalendarioAcademico.Data.DBContext.Repositories.Generic
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly CalendarioAcademicoContext _context;
        protected readonly DbSet<TEntity> _entities;
        public GenericRepository(CalendarioAcademicoContext context)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
            _entities = context.Set<TEntity>();
        }
        #region Atualizar
        public virtual async Task<bool> Atualizar(TEntity entity)
        {
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                return await Task.FromResult(true);
            }
            catch
            {
                return await Task.FromResult(false);
            }
        }
        #endregion
        #region Buscar por ID
        public TEntity BuscarPorId(int id)
        {
            return _entities.Find(id);
        }
        #endregion
        #region Buscar por ID Assíncrono
        public async Task<TEntity> BuscarPorIdAsync(int id)
        {
            return await _entities.FindAsync(id);
        }
        #endregion
        #region Cadastrar Assíncrono
        public async Task<TEntity> CadastrarAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            await _entities.AddAsync(entity);
            return entity;
        }
        #endregion
        #region Desativar Assíncrono
        public async Task<TEntity> DesativarAsync(int id)
        {
            var entity = await BuscarPorIdAsync(id);
            if (entity == null)
                throw new ArgumentException($"Entidade com ID {id} não encontrada.");

            var entityType = typeof(TEntity).Name;

            switch (entityType)
            {
                case "CAD_Calendario":
                    var statusCAD = entity.GetType().GetProperty("CAD_Status");
                    if (statusCAD != null)
                    {
                        statusCAD.SetValue(entity, StatusCalendario.Desativado);
                        _entities.Update(entity);
                    }
                    else
                    {
                        throw new InvalidOperationException("Propriedade StatusCalendario não encontrada.");
                    }
                    break;

                case "EVNT_Evento":
                    var statusEVNT = entity.GetType().GetProperty("EVNT_Ativo");
                    if (statusEVNT != null && statusEVNT.PropertyType == typeof(bool))
                    {
                        statusEVNT.SetValue(entity, false);
                        _entities.Update(entity);
                    }
                    else
                    {
                        throw new InvalidOperationException("Propriedade EVNT_Ativo não encontrada.");
                    }
                    break;
                case "PORT_Portaria":
                    var statusPORT = entity.GetType().GetProperty("PORT_Ativo");
                    if (statusPORT != null && statusPORT.PropertyType == typeof(bool))
                    {
                        statusPORT.SetValue(entity, false);
                        _entities.Update(entity);
                    }
                    else
                    {
                        throw new InvalidOperationException("Propriedade PORT_Portaria não encontrada");
                    }
                    break;
                default:
                    _entities.Remove(entity);
                    break;
            }
            return entity;
        }
        #endregion
        #region Listar
        public IEnumerable<TEntity> Listar()
        {
            return _entities.AsEnumerable();
        }
        #endregion
        #region Listar por Condição
        public async Task<IEnumerable<TEntity>> Listar_Por_Condicao(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _entities;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.Where(expression).ToListAsync();
        }
        #endregion
        #region Listar Assíncrono
        public async Task<IEnumerable<TEntity>> ListarAsync(params Expression<Func<TEntity, object>>[] includes)
        {
            if (includes is null)
            {
                throw new ArgumentNullException(nameof(includes));
            }
            IQueryable<TEntity> query = _entities;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.ToListAsync();
        }
        #endregion
        #region Paginação com Filtro
        public async Task<IEnumerable<TEntity>> Listar_Paginacao_Com_Filtro(Expression<Func<TEntity, bool>> filtro = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string incluirPropriedades = "", int? tamanhoPagina = null, int? numeroPagina = null)
        {
            IQueryable<TEntity> query = _entities.AsNoTracking();
            if (filtro is not null) query = query.Where(filtro);
            if (orderBy is not null) query = orderBy(query);
            foreach (var prop in incluirPropriedades.Split(
                [','], StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(prop);
            }
            return (numeroPagina is not null && numeroPagina is not null) ? query.Skip((numeroPagina.Value - 1) * tamanhoPagina.Value).Take(tamanhoPagina.Value) : query;
        }
        #endregion
        #region Paginação
        public async Task<PaginacaoDTO<TEntity>> Listar_Por_Paginacao(Expression<Func<TEntity, bool>> filtro = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string incluirPropriedades = " ",
            int tamanhoPagina = 10,
            int numeroPagina = 1)
        {
            if(tamanhoPagina <= 0)
            {
                throw new ArgumentException("O tamanho da página deve ser maior que zero.", nameof(tamanhoPagina));
            }
            if(numeroPagina <= 0)
            {
                throw new ArgumentException("O número da página deve ser maior que zero.", nameof(numeroPagina));
            }
            IQueryable<TEntity> query = _entities.AsNoTracking();
            if(filtro != null)
            {
                query = query.Where(filtro);
            }
            int totalRegistros = await query.CountAsync();
            if(orderBy != null)
            {
                query = orderBy(query);
            }
            foreach (var prop in incluirPropriedades.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(prop.Trim());
            }

            var itens = await query
                .Skip((numeroPagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamanhoPagina);
            return new PaginacaoDTO<TEntity>
            {
                Itens = itens,
                TotalRegistros = totalRegistros,
                TotalPaginas = totalPaginas,
                PaginaAtual = numeroPagina,
                TamanhoPagina = tamanhoPagina
            };

        }
        #endregion
    }
}