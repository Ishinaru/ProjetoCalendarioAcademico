using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CalendarioAcademico.Domain.Models;

public partial class PORT_Portaria
{
    [Key]
    public int PORT_CD_Portaria { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? PORT_NumPortaria { get; set; }

    public int? PORT_Ano { get; set; }

    public bool? PORT_Ativo { get; set; }

    public string? PORT_Observacao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PORT_DT_DataAtualizacao { get; set; }

    public int? PORT_CD_Usuario { get; set; }

    [InverseProperty("EVPT_CD_PortariaNavigation")]
    public virtual ICollection<EVPT_Evento_Portaria> EVPT_Evento_Portaria { get; set; } = new List<EVPT_Evento_Portaria>();
}
