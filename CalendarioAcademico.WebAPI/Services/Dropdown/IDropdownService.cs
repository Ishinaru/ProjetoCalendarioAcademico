using Microsoft.AspNetCore.Mvc.Rendering;

namespace CalendarioAcademico.WebAPI.Services.Dropdown
{
    public interface IDropdownService
    {
        Task<SelectList> Gerar_SelectList<TEntity>(
            Func<Task<IEnumerable<TEntity>>> recuperar_dados,
            Func<TEntity, string> seletor_valor,
            Func<TEntity, string> seletor_texto,
            string? defaultOption = null
            ) where TEntity : class;
        Task<SelectList> Gerar_SelectList_Enum<TEnum>(string defaultOption = null) where TEnum : struct, Enum;
    }
}
