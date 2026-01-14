using Likano.Application.DTOs;
using Likano.Application.Features.Manage.Product.Queries.GetAll;

namespace Likano.Web.Models.Manage
{
    public class ProductsManageViewModel
    {
        public ProductsFilterVm Filter { get; set; } = new();
        public GetAllProductsForManageResponse? Response { get; set; }
        public List<CategoryDtoForManage> Categories { get; set; } = new();
        public List<BrandDtoForManage> Brands { get; set; } = new();
        public List<ProducerCountryDtoForManage> ProducerCountries { get; set; } = new();
        bool? IntoGrid { get; set; } = true;
    }
}