using CalendarioAcademico.Data.DBContext.Repositories.Generic;
using CalendarioAcademico.Domain.Models;

namespace CalendarioAcademico.Data.DBContext.Repositories.Calendario
{
    public interface ICalendarioRepository : IGenericRepository<CAD_Calendario>
    {
        Task<IEnumerable<CAD_Calendario>> Consultar_Aprovados();
        Task<IEnumerable<CAD_Calendario>> Consultar_Por_Ano(int ano_calendario);

    }
}
