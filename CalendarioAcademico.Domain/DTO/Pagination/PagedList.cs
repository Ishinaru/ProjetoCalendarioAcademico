namespace CalendarioAcademico.Domain.DTO.Pagination
{
    public class PagedList<T> : List<T>
    {
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public int TamanhoPagina { get; set; }
        public int TotalRegistros { get; set; }
        public PagedList(IEnumerable<T> items, int paginaAtual, int tamanhoDaPagina, int count)
        {
            PaginaAtual = paginaAtual;
            TotalPaginas = (int)Math.Ceiling(count / (double)tamanhoDaPagina);
            TamanhoPagina = tamanhoDaPagina;
            TotalRegistros = count;

            AddRange(items);
        }
    }
}
