using CalendarioAcademico.Data.DBContext.Repositories.Generic;
using CalendarioAcademico.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarioAcademico.Data.DBContext.Repositories.Portaria
{
    public class PortariaRepository : GenericRepository<PORT_Portaria>, IPortariaRepository
    {
        private readonly IDbContextFactory<CalendarioAcademicoContext> _dbContextFactory;
        public PortariaRepository(CalendarioAcademicoContext dbContext, IDbContextFactory<CalendarioAcademicoContext> dbContextFactory) : base(dbContext)
        {
            _dbContextFactory = dbContextFactory;
        }
    }
}
