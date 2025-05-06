namespace CalendarioAcademico.Domain.DTO.Pagination
{
    public class PaginacaoDTO<T>
    {
        public List<T> Itens { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; }
        public int PaginaAtual { get; set; }
        public int TamanhoPagina { get; set; }
    }
}
