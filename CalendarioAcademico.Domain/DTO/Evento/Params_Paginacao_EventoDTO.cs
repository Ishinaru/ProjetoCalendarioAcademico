using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CalendarioAcademico.Domain.DTO.Evento
{
    public class Params_Paginacao_EventoDTO
    {
        [Range(1, 50, ErrorMessage = "O tamanho da página deve ser entre 1 e 50.")]
        [DefaultValue(10)]
        public int TamanhoPagina { get; set; } = 10;
        [Range(1, int.MaxValue, ErrorMessage = "O número da página deve ser maior ou igual a 1.")]
        [DefaultValue(1)]
        public int NumeroPagina { get; set; } = 1;
        public int? CalendarioId { get; set; }
        // Tipo de feriado para filtrar eventos (ex.: 0 = Não feriado, 1 = Nacional, 2 = Municipal) (opcional).
        public int? TipoFeriado { get; set; }
        public string? OrdenarPor { get; set; }
        public string? Ordem { get; set; }
        public string? IncluirRelacionamentos { get; set; }
    }
}
