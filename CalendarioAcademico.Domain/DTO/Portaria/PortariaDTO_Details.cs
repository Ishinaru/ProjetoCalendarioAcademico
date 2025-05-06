using CalendarioAcademico.Domain.DTO.EventoPortaria;

namespace CalendarioAcademico.Domain.DTO.Portaria
{
    public class PortariaDTO_Details
    {
        public int id { get; set; }
        public string? num_portaria { get; set; }
        public int? ano { get; set; }
        public bool? is_ativo { get; set; }
        public string? observacao { get; set; }
        public DateTime? data_atualizacao { get; set; }
        public int? id_usuario { get; set; }
        public List<EventoPortariaDTO> eventos { get; set; } = new List<EventoPortariaDTO>();
    }
}
