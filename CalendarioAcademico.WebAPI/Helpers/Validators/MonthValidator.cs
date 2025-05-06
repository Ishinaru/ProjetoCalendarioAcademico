using System.Globalization;

namespace CalendarioAcademico.WebAPI.Helpers.Validators
{
    public class MonthValidator
    {
        public static string ObterNomeDoMes(int mes)
        {
            if (mes < 1 || mes > 12) return "Mês inválido";
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(mes);
        }
    }
}
