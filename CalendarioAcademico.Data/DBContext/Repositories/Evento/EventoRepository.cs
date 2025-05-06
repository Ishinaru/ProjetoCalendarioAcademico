using CalendarioAcademico.Data.DBContext.Repositories.Generic;
using CalendarioAcademico.Data.Helpers;
using CalendarioAcademico.Domain.DTO.Pagination;
using CalendarioAcademico.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarioAcademico.Data.DBContext.Repositories.Evento
{
    public class EventoRepository : GenericRepository<EVNT_Evento>, IEventoRepository
    {
        private readonly IDbContextFactory<CalendarioAcademicoContext> _dbContextFactory;
        public EventoRepository(CalendarioAcademicoContext context, IDbContextFactory<CalendarioAcademicoContext> dbContextFactory) : base(context)
        {
            _dbContextFactory = dbContextFactory;
        }
        #region Evento por Ano
        public async Task<ICollection<EVNT_Evento>> Evento_Por_Ano(int ano)
        {
            return await _entities.Include(c => c.EVNT_CD_CalendarioNavigation)
                .Where(c => c.EVNT_CD_CalendarioNavigation.CAD_Ano == ano)
                .ToListAsync();
        }
        #endregion
        #region Evento por Mês
        public async Task<ICollection<EVNT_Evento>> Evento_Por_Mes(int anoCalendario, int mes)
        {
            // Define o intervalo do mês usando DateOnly
            var inicioMes = new DateOnly(anoCalendario, mes, 1);
            var ultimoDiaMes = inicioMes.AddMonths(1).AddDays(-1);
            var fimMes = new DateOnly(anoCalendario, mes, ultimoDiaMes.Day);

            return await _entities
                .Include(e => e.EVNT_CD_CalendarioNavigation)
                .Where(e =>
                    e.EVNT_CD_CalendarioNavigation.CAD_Ano == anoCalendario &&
                    (
                        // Evento começa dentro do mês
                        (e.EVNT_DT_DataInicio.HasValue &&
                         e.EVNT_DT_DataInicio >= inicioMes &&
                         e.EVNT_DT_DataInicio <= fimMes) ||

                        // Evento termina dentro do mês
                        (e.EVNT_DT_DataFinal.HasValue &&
                         e.EVNT_DT_DataFinal >= inicioMes &&
                         e.EVNT_DT_DataFinal <= fimMes) ||

                        // Evento abrange o mês completamente
                        (e.EVNT_DT_DataInicio.HasValue &&
                         e.EVNT_DT_DataFinal.HasValue &&
                         e.EVNT_DT_DataInicio < inicioMes &&
                         e.EVNT_DT_DataFinal > fimMes)
                    )
                )
                .OrderBy(e => e.EVNT_DT_DataInicio)
                .ToListAsync();
        }
        #endregion
        #region Evento por Calendário
        public async Task<ICollection<EVNT_Evento>> Evento_Por_Calendario(int idCalendario)
        {
            return await _entities.Where(e => e.EVNT_CD_Calendario == idCalendario).ToListAsync();
        }
        #endregion
        #region Evento por Periodo
        public async Task<ICollection<EVNT_Evento>> Evento_Por_Periodo(DateOnly dataInicio, DateOnly dataFinal)
        {
            return await _entities
                .Include(e => e.EVNT_CD_CalendarioNavigation)
                .Where(e => e.EVNT_DT_DataInicio <= dataFinal && e.EVNT_DT_DataFinal >= dataInicio)
                .OrderBy(e => e.EVNT_DT_DataInicio)
                .ToListAsync();
        }
        #endregion
        #region Paginação de Eventos
        public async Task<PagedList<EVNT_Evento>> Selecionar_Por_Paginacao(int numeroPagina, int tamanhoPagina)
        {
            var query = _context.EVNT_Evento.AsQueryable();
            return await PaginationHelper.CriarAsync(query, numeroPagina, tamanhoPagina);
        }
        #endregion
    }
}