namespace CalendarioAcademico.Domain.DTO.Evento
{
    public class EventoDTO_View
    {
        public int IdEvento { get; set; }
        public string Descricao { get; set; }
        public DateOnly? DataInicio { get; set; }
        public DateOnly? DataFinal { get; set; }
        public string Origem { get; set; } // Indica se as datas vêm da portaria ou do evento original
    }
}
