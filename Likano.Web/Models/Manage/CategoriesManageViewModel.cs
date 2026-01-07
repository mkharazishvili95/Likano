using Likano.Application.Features.Manage.Category.Queries.GetAll;

namespace Likano.Web.Models.Manage
{
    public class CategoriesManageViewModel
    {
        public CategoriesFilterVm Filter { get; set; } = new();
        public GetAllCategoriesForManageResponse? Response { get; set; }
        public bool? IntoGrid { get; set; } = true;
    }
}
