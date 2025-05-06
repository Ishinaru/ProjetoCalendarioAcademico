using CalendarioAcademico.Data.DBContext.Repositories.Generic;
using CalendarioAcademico.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarioAcademico.Data.DBContext.Repositories.EventoPortaria
{
    public class EventoPortariaRepository : GenericRepository<EVPT_Evento_Portaria>, IEventoPortariaRepository
    {
        private readonly IDbContextFactory<CalendarioAcademicoContext> _dbContextFactory;
        public EventoPortariaRepository(CalendarioAcademicoContext dbContext, IDbContextFactory<CalendarioAcademicoContext> dbContextFactory):base(dbContext)
        {
            _dbContextFactory = dbContextFactory;
        }
        #region Deletar
        //Deletar referência para uma portaria e evento
        public async Task<bool> Deletar(EVPT_Evento_Portaria entity)
        {
            try
            {
                _entities.Remove(entity);
                return await Task.FromResult(true);

            }
            catch (Exception)
            {
                return await Task.FromResult(false);
            }
        }
        #endregion
    }
}