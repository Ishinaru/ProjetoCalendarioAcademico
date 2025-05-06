namespace CalendarioAcademico.Domain.DTO.Evento
{
    public class EventoDTO_Edit
    {
        public DateOnly? DataInicio { get; set; }
        public DateOnly? DataFinal { get; set; }
        public string? Descricao { get; set; }
        public bool? UescFunciona { get; set; }
        public bool? Importante { get; set; }
        public bool? Desativar { get; set; }
        public int? TipoFeriado { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
}
