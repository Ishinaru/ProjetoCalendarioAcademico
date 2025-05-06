namespace CalendarioAcademico.Domain.DTO.Evento
{
    public class EventoDTO_Details
    {
        public int id { get; set; }
        public DateOnly? data_inicio { get; set; }
        public DateOnly? data_final { get; set; }
        public string? descricao { get; set; }
        public bool? uesc_funciona { get; set; }
        public bool? eh_importante { get; set; }
        public bool? ativo { get; set; }
        public int? tipo_de_feriado { get; set; }
        public DateTime? data_atualizacao { get; set; }
        public int? id_calendario { get; set; }
    }
}
