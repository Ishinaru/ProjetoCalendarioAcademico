using CalendarioAcademico.Domain.Enum;
using CalendarioAcademico.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CalendarioAcademico.WebAPI.Helpers.Validators
{
    public class CalendarioValidations
    {
        public static IActionResult? Validar_Calendario(CAD_Calendario calendario, int idCalendario)
        {
            if(calendario == null)
            {
                return new NotFoundObjectResult($"Calendário com ID {idCalendario} não encontrado.");
            }

            if(calendario.CAD_Status == StatusCalendario.Desativado)
            {
                return new BadRequestObjectResult("Não é possível completar a ação devido ao status do calendário : desativado.");
            }

            if(calendario.CAD_Status == StatusCalendario.Aprovado)
            {
                return new BadRequestObjectResult("Não é possível completar a ação de edição devido ao status do calendário : aprovado.");
            }

            return null;
        }
    }
}
