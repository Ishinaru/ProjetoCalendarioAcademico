using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CalendarioAcademico.Domain.Helpers.Validators;

namespace CalendarioAcademico.Domain.DTO.Calendario
{
    [Display(Name = "Calendário")]
    public class CalendarioDTO_Cadastrar
    {
        [Validar_Ano_Dinamicamente(20)]
        [Required(ErrorMessage = "O campo ano é obrigatório e não pode ser nulo.")]
        public int ano { get; set; }
        [StringLength(500, ErrorMessage = "A observação não deve exceder 500 caracteres.")]
        public string? observacao { get; set; }
    }
}
