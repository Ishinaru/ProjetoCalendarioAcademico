using CalendarioAcademico.Data.DBContext.UnitOfWork;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CalendarioAcademico.WebAPI.Services.Dropdown
{
    public class DropdownService : IDropdownService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public DropdownService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #region Gerar SelectList
        public async Task<SelectList> Gerar_SelectList<TEntity>(Func<Task<IEnumerable<TEntity>>> recuperar_dados, Func<TEntity, string> seletor_valor, Func<TEntity, string> seletor_texto, string? defaultOption = null) where TEntity : class
        {
            var data = await recuperar_dados();
            var items = data.Select(data => new SelectListItem
            {
                Value = seletor_valor(data),
                Text = seletor_texto(data)
            })
            .OrderBy(item => item.Text)
            .ToList();

            if (!string.IsNullOrEmpty(defaultOption)) items.Insert(0, new SelectListItem(defaultOption, ""));

            return new SelectList(items, "Value", "Text");
        }
        #endregion
        #region Gerar SelectList para Enum
        public async Task<SelectList> Gerar_SelectList_Enum<TEnum>(string defaultOption = null) where TEnum : struct, Enum
        {
            var values = Enum.GetValues<TEnum>();
            var items = values
                .Select(e => new SelectListItem
                {
                    Value = Convert.ToInt32(e).ToString(),
                    Text = GetDisplayName(e)
                })
                .OrderBy(item => item.Text)
                .ToList();

            if (!string.IsNullOrEmpty(defaultOption)) items.Insert(0, new SelectListItem(defaultOption, ""));

            return new SelectList(items, "Value", "Text");
        }
        #endregion
        #region Get Display name
        private string GetDisplayName(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = Attribute.GetCustomAttribute
            (
                field,
                typeof(DisplayAttribute)
            ) as DisplayAttribute;
            return attribute?.Name ?? value.ToString();
        }
        #endregion
    }
}
