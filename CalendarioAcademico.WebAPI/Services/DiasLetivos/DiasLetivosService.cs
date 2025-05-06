
using CalendarioAcademico.Data.DBContext.Repositories.Evento;
using CalendarioAcademico.Data.DBContext.UnitOfWork;
using CalendarioAcademico.Domain.Models;

namespace CalendarioAcademico.WebAPI.Services.DiasLetivos
{
    public class DiasLetivosService : IDiasLetivosService
    {
        private readonly IUnitOfWork _context;
        public DiasLetivosService(IUnitOfWork uwo_context)
        {
            _context = uwo_context;
        }
        #region Função para Contar Dias Letivos por Ano
        public async Task<int> Contar_Dias_Letivos_Ano(int ano)
        {
            int diasLetivos = 0;
            for (int mes = 1; mes <= 12; mes++)
            {
                diasLetivos += await Contar_Dias_Letivos_Mes(ano, mes);
            }
            return diasLetivos;
        }
        #endregion
        #region Função para Contar Dias Letivos por Mês
        public async Task<int> Contar_Dias_Letivos_Mes(int ano, int mes)
        {
            if (mes < 1 || mes > 12)
            {
                throw new ArgumentException("Mês inválido. Mês deve estar entre 1 e 12", nameof(mes));
            };

            int diasNoMes = DateTime.DaysInMonth(ano, mes);
            int diasLetivos = 0;

            for (int dia = 1; dia <= diasNoMes; dia++)
            {
                var data = new DateOnly(ano, mes, dia);
                if (await Eh_Dia_Letivo(data))
                {
                    diasLetivos++;
                }
            }
            return diasLetivos;
        }
        #endregion
        #region Função para Indicar se é Dia Letivo
        private async Task<bool> Eh_Dia_Letivo(DateOnly data)
        {
            if(data.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            var eventoNoDia = (await _context.GetRepository<EventoRepository, EVNT_Evento>()
                .Listar_Por_Condicao(e => e.EVNT_DT_DataInicio <= data && e.EVNT_DT_DataFinal >= data))
                .FirstOrDefault()
                ;
            if (eventoNoDia == null) return true;

            return eventoNoDia.EVNT_UescFunciona == true;
        }
        #endregion
    }
}