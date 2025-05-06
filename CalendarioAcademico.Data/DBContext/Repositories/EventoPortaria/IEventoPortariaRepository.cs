using CalendarioAcademico.Data.DBContext.Repositories.Generic;
using CalendarioAcademico.Domain.Models;

namespace CalendarioAcademico.Data.DBContext.Repositories.EventoPortaria
{
    public interface IEventoPortariaRepository : IGenericRepository<EVPT_Evento_Portaria>
    {
        Task<bool> Deletar(EVPT_Evento_Portaria entity);
    }
}
