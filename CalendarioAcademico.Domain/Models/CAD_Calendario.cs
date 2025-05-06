using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CalendarioAcademico.Domain.Enum;
using Microsoft.EntityFrameworkCore;

namespace CalendarioAcademico.Domain.Models;

[Index("CAD_Ano", Name = "UQ__CAD_Cale__8448D96D4FFD3E11", IsUnique = true)]
public partial class CAD_Calendario
{
    [Key]
    public int CAD_CD_Calendario { get; set; }

    public int? CAD_Ano { get; set; }

    public StatusCalendario CAD_Status { get; set; }

    public string? CAD_DS_Observacao { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? CAD_NumeroResolucao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CAD_DT_DataAtualizacao { get; set; }

    public int? CAD_CD_Usuario { get; set; }

    public int? CAD_CD_Evento { get; set; }

    [InverseProperty("EVNT_CD_CalendarioNavigation")]
    public virtual ICollection<EVNT_Evento> EVNT_Evento { get; set; } = new List<EVNT_Evento>();
}
