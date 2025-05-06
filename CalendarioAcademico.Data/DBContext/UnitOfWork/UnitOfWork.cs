using CalendarioAcademico.Data.DBContext.Repositories.Generic;
using CalendarioAcademico.Domain.Models;
using Microsoft.EntityFrameworkCore.Storage;
using CalendarioAcademico.Data.DBContext.Repositories.Calendario;
using CalendarioAcademico.Data.DBContext.Repositories.Evento;
using Microsoft.EntityFrameworkCore;
using CalendarioAcademico.Data.DBContext.Repositories.Portaria;
using CalendarioAcademico.Data.DBContext.Repositories.EventoPortaria;

namespace CalendarioAcademico.Data.DBContext.UnitOfWork
{
    
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly IDbContextFactory<CalendarioAcademicoContext> _dbContextFactory;
        private readonly CalendarioAcademicoContext _context;
        private readonly Dictionary<Type, object> _repositories;
        private IDbContextTransaction _transaction;
        private bool _disposed = false;

        public UnitOfWork(CalendarioAcademicoContext context, IDbContextFactory<CalendarioAcademicoContext> dbContextFactory)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _repositories = new Dictionary<Type, object>();

            _repositories[typeof(CAD_Calendario)] = new CalendarioRepository(_context, _dbContextFactory);
            _repositories[typeof(EVNT_Evento)] = new EventoRepository(_context, _dbContextFactory);
            _repositories[typeof(PORT_Portaria)] = new PortariaRepository(_context, _dbContextFactory);
            _repositories[typeof(EVPT_Evento_Portaria)] = new EventoPortariaRepository(_context, _dbContextFactory);
        }
        #region BeginTransaction Assíncrono
        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
                throw new InvalidOperationException("Já existe uma transação ativa.");

            _transaction = await _context.Database.BeginTransactionAsync();
        }
        #endregion
        #region Commit Assíncrono
        public async Task CommitAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("Nenhuma transação ativa para commit.");
            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch (Exception)
            {
                await _transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }
        #endregion
        #region Função Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            
            if(disposing)
            {
                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _transaction = null;
                }

                _context.Dispose();
            }
            _disposed = true;
        }
        #endregion
        #region Get Repository
        public TRepo? GetRepository<TRepo, TEntity>() where TRepo : class where TEntity : class
        {
            var type = typeof(TEntity);
            if(!_repositories.ContainsKey(type))
            {
                throw new InvalidOperationException($"Repositório para {type.Name} não foi registrado.");
            }
            return _repositories[type] as TRepo;
        }
        #endregion
        #region Rollback Assíncrono
        public async Task RollbackAsync()
        {
            if (_transaction == null)
                return;

            try
            {
                await _transaction.RollbackAsync();
            }

            finally
            {
                await DisposeTransactionAsync();
            }
        }
        #endregion
        #region Save Changes Assíncrono
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        #endregion
        #region Dispose Transaction Assíncrono
        private async Task DisposeTransactionAsync()
        {
            if(_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
        #endregion
    }
}