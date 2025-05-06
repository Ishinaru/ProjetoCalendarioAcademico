using CalendarioAcademico.Domain.DTO.EventoPortaria;

namespace CalendarioAcademico.Domain.DTO.Portaria
{
    public class PortariaDTO_Edit
    {
        public string ? num_portaria { get; set; }
        public bool? desativar { get; set; }
        public string ? observacao { get; set; }
        public DateTime? data_atualizacao { get; set; }
        public List<EventoPortariaDTO>? eventos { get; set; } = new List<EventoPortariaDTO>();
    }
}
