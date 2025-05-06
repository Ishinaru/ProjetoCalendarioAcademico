using System.ComponentModel.DataAnnotations;

namespace CalendarioAcademico.Domain.DTO.Calendario
{
    [Display(Name = "Calendário")]
    public class CalendarioDTO_Details
    {
        [Key]
        [Display(Name = "id")]
        public int id { get; set; } //cad_cd_calendario
        [Display(Name = "ano")]
        public int ano { get; set; } //cad_ano
        [Display(Name = "observação")]
        public string ? observacao { get; set; } //cad_ds_observacao
        [Display(Name = "status")]
        public int status { get; set; } //cad_status
        [Display(Name = "número de resolução")]
        public string ? num_resolucao { get; set; } //cad_numeroResolucao
        [Display(Name = "data de atualização")]
        public DateTime data_atualizacao { get; set; } //cad_dt_dataAtualizacao
        
        //Objetos Relacionados
        //[Display(Name = "eventos")]

    }
}
