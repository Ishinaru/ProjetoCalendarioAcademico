using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CalendarioAcademico.Domain.Models;

[PrimaryKey("EVPT_CD_Evento", "EVPT_CD_Portaria")]
public partial class EVPT_Evento_Portaria
{
    [Key]
    public int EVPT_CD_Evento { get; set; }

    [Key]
    public int EVPT_CD_Portaria { get; set; }

    public DateOnly? EVPT_DT_DataInicio { get; set; }

    public DateOnly? EVPT_DT_DataFinal { get; set; }

    public string? EVPT_Observacao { get; set; }

    public bool EVPT_Ativo { get; set; }

    [ForeignKey("EVPT_CD_Evento")]
    [InverseProperty("EVPT_Evento_Portaria")]
    public virtual EVNT_Evento EVPT_CD_EventoNavigation { get; set; } = null!;

    [ForeignKey("EVPT_CD_Portaria")]
    [InverseProperty("EVPT_Evento_Portaria")]
    public virtual PORT_Portaria EVPT_CD_PortariaNavigation { get; set; } = null!;
}
