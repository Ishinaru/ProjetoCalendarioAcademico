using CalendarioAcademico.Data.DBContext.Repositories.Generic;
using CalendarioAcademico.Domain.Enum;
using CalendarioAcademico.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarioAcademico.Data.DBContext.Repositories.Calendario
{
    public class CalendarioRepository : GenericRepository<CAD_Calendario>, ICalendarioRepository
    {
        private readonly IDbContextFactory<CalendarioAcademicoContext> _dbContextFactory;

        public CalendarioRepository(CalendarioAcademicoContext dbContext, IDbContextFactory<CalendarioAcademicoContext> dbContextFactory) : base(dbContext)
        {
            _dbContextFactory = dbContextFactory;
        }
        #region Consultar Aprovados
        public async Task<IEnumerable<CAD_Calendario>> Consultar_Aprovados()
        {

            using (var cadDbContext = _dbContextFactory.CreateDbContext())
            {
                try
                {
                    IEnumerable<CAD_Calendario> calendarios = await cadDbContext.CAD_Calendario
                        .Where(c => c.CAD_Status == StatusCalendario.Aprovado)
                        .ToListAsync();
                    return calendarios;
                }
                catch (Exception)
                {
                    return null;
                }

            }
        }
        #endregion
        #region Consultar por Ano
        public async Task<IEnumerable<CAD_Calendario>> Consultar_Por_Ano(int ano_calendario)
        {
            using (var cadDbContext = _dbContextFactory.CreateDbContext())
            {
                try
                {
                    IEnumerable<CAD_Calendario> calendarios = await cadDbContext.CAD_Calendario
                        .Where(c => c.CAD_Ano == ano_calendario)
                        .ToListAsync();
                    return calendarios;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        #endregion
    }
}