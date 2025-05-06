using CalendarioAcademico.Domain.DTO.EventoPortaria;

namespace CalendarioAcademico.Domain.DTO.Portaria
{
    public class PortariaDTO_Cadastrar
    {
        public string? num_portaria { get; set; }
        public string? observacao { get; set; }
        public int id_calendario { get; set; }
        public List<EventoPortariaDTO> eventos { get; set; } = new List<EventoPortariaDTO>();
    }
}
