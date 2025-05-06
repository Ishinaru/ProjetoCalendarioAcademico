using CalendarioAcademico.Domain.DTO.Calendario;

namespace CalendarioAcademico.Domain.DTO.Evento
{
    public class EventoDTO_Paginacao_View
    {
        public int IdEvento { get; set; }
        public string Descricao { get; set; }
        public DateOnly? DataInicio { get; set; }
        public DateOnly? DataFinal { get; set; }
        public bool Ativo { get; set; }
        public CalendarioDTO_View_Evento Calendario { get; set; }
    }
}
