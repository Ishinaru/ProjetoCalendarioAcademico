using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalendarioAcademico.Domain.Models;

public partial class EVNT_Evento
{
    [Key]
    public int EVNT_CD_Evento { get; set; }

    public DateOnly? EVNT_DT_DataInicio { get; set; }

    public DateOnly? EVNT_DT_DataFinal { get; set; }

    public string? EVNT_DS_Descricao { get; set; }

    public bool? EVNT_UescFunciona { get; set; }

    public bool? EVNT_Importante { get; set; }

    public bool? EVNT_Ativo { get; set; }

    public int? EVNT_TipoFeriado { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EVNT_DT_DataAtualizacao { get; set; }

    public int? EVNT_CD_Usuario { get; set; }

    public int? EVNT_CD_Calendario { get; set; }

    [ForeignKey("EVNT_CD_Calendario")]
    [InverseProperty("EVNT_Evento")]
    public virtual CAD_Calendario? EVNT_CD_CalendarioNavigation { get; set; }

    [InverseProperty("EVPT_CD_EventoNavigation")]
    public virtual ICollection<EVPT_Evento_Portaria> EVPT_Evento_Portaria { get; set; } = new List<EVPT_Evento_Portaria>();
}
