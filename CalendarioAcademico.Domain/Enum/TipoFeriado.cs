using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CalendarioAcademico.Domain.Enum
{
    public enum TipoFeriado
    {
        [Display(Name = "Não Feriado")]
        Não_Feriado = 0,

        [Display(Name = "Feriado Municipal")]
        Municipal = 1,

        [Display(Name = "Feriado Estadual")]
        Estadual = 2,

        [Display(Name = "Feriado Nacional")]
        Nacional = 3
    }
}
