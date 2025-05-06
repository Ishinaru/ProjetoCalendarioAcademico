using CalendarioAcademico.Domain.DTO.Pagination;
using Microsoft.EntityFrameworkCore;

namespace CalendarioAcademico.Data.Helpers
{
    public static class PaginationHelper
    {
        public static async Task<PagedList<T>> CriarAsync<T>(IQueryable<T> source, int numeroDaPagina, int tamanhoPagina) where T : class
        {
            var quantidadeItems = await source.CountAsync();
            var items = await source.Skip((numeroDaPagina - 1 ) * tamanhoPagina).Take((tamanhoPagina)).ToListAsync();
            return new PagedList<T>(items, numeroDaPagina, tamanhoPagina, quantidadeItems);
        }
    }
}
