using CalendarioAcademico.Domain.Helpers.Validators;
using System.ComponentModel.DataAnnotations;

namespace CalendarioAcademico.Domain.DTO.Calendario
{
    public class CalendarioDTO_Edit
    {
        [StringLength(500, ErrorMessage = "A observação não deve exceder 500 caracteres.")]
        public string? observacao { get; set; }

        public bool ? desativar { get; set; }

    }
}
