using Microsoft.AspNetCore.Mvc;

namespace CalendarioAcademico.WebAPI.Helpers.Validators
{
    public class EventoValidations
    {
        public static IActionResult? Validar_Data(DateOnly? dataInicio, DateOnly? dataFinal)
        {
            if(dataInicio.HasValue && dataFinal.HasValue && dataFinal < dataInicio)
            {
                return new BadRequestObjectResult("A data final não pode ser anterior á data inicial.");
            }
            return null;
        }


    }
}
