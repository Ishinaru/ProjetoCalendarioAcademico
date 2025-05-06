using System.ComponentModel.DataAnnotations;

namespace CalendarioAcademico.Domain.Helpers.Validators
{
    public class Validar_Ano_Dinamicamente : ValidationAttribute
    {
        private readonly int _anos_futuros;

        public Validar_Ano_Dinamicamente(int anos_futuros)
        {
            _anos_futuros = anos_futuros;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value == null)
            {
                return new ValidationResult("O ano é obrigatório.");
            }

            int anoAtual = DateTime.Now.Year;
            int anoMaximo = anoAtual + _anos_futuros;

            if (int.TryParse(value.ToString(), out int ano))
            {
                if(ano < anoAtual)
                {
                    return new ValidationResult($"O ano não pode ser anterior a {anoAtual}.");
                }
                if(ano > anoMaximo)
                {
                    return new ValidationResult($"O ano não pode ser posterior a {anoMaximo}.");
                }
                return ValidationResult.Success;
            }

            return new ValidationResult("O ano deve ser um número válido.");
        }
    }
}
