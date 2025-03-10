using System.ComponentModel.DataAnnotations;

namespace CalendarioAcademico.Domain.Enum
{
    public enum StatusCalendario
    {
        [Display(Name = "Calendário Aguardando Aprovação")]
        Aguardando_Aprovacao = 0,

        [Display(Name = "Calendário Aprovado")]
        Aprovado = 1,

        [Display(Name = "Calendário Desativado")]
        Desativado = 2
    }

}
