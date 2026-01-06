using Likano.Application.Features.Manage.Product.Queries.GetAll;

namespace Likano.Web.Models.Manage
{
    public class ProductsManageViewModel
    {
        public ProductsFilterVm Filter { get; set; } = new();
        public GetAllProductsForManageResponse? Response { get; set; }
    }
}