using CalendarioAcademico.Domain.Enum;

namespace CalendarioAcademico.Domain.DTO.Evento
{
    public class EventoDTO_Cadastrar
    {
        public DateOnly DataInicio { get; set; }
        public DateOnly DataFinal { get; set; }
        public string? Descricao { get; set; }
        public bool UescFunciona { get; set; }
        public bool Importante { get; set; }
        public TipoFeriado TipoFeriado { get; set; }
    }
}
