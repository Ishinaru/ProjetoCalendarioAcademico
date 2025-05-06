using System.ComponentModel.DataAnnotations;

namespace CalendarioAcademico.Domain.DTO.Calendario
{
    public class CalendarioDTO_View_Evento
    {
        public int IdCalendario { get; set; } //cad_cd_calendario
        public int Ano { get; set; } //cad_ano
        public string? Observacao { get; set; } //cad_ds_observacao
        public int Status { get; set; } //cad_status
        [Display(Name = "Número de Resolução")]
        public string? NumResolucao { get; set; } //cad_numeroResolucao
    }
}
