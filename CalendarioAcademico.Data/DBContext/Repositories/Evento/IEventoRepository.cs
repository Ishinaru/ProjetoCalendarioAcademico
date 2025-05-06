using CalendarioAcademico.Data.DBContext.Repositories.Generic;
using CalendarioAcademico.Domain.DTO.Pagination;
using CalendarioAcademico.Domain.Models;

namespace CalendarioAcademico.Data.DBContext.Repositories.Evento
{
    public interface IEventoRepository : IGenericRepository<EVNT_Evento>
    {
        Task<ICollection<EVNT_Evento>> Evento_Por_Calendario(int idCalendario);
        Task<ICollection<EVNT_Evento>> Evento_Por_Ano(int ano);
        Task<ICollection<EVNT_Evento>> Evento_Por_Mes(int idCalendario, int mes);
        Task<ICollection<EVNT_Evento>> Evento_Por_Periodo(DateOnly dataInicio, DateOnly dataFinal);
        Task<PagedList<EVNT_Evento>> Selecionar_Por_Paginacao(int numeroPagina, int tamanhoPagina);
    }
}
