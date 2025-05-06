namespace CalendarioAcademico.Domain.DTO.EventoPortaria
{
    public class EventoPortariaDTO
    {
        public int ? id_evento { get; set; }
        public DateOnly? nova_data_inicio { get; set; }
        public DateOnly? nova_data_final { get; set; }
    }
}
