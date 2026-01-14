using Likano.Application.DTOs;
using Likano.Application.Features.Manage.Product.Queries.Get;

namespace Likano.Web.Models.Manage
{
    public class ProductDetailsViewModel
    {
        public GetProductForManageResponse Product { get; set; } = new();
        public List<CategoryDtoForManage> Categories { get; set; } = new();
        public List<BrandDtoForManage> Brands { get; set; } = new();
        public List<ProducerCountryDtoForManage> Countries { get; set; } = new();
    }
}
