using Likano.Application.DTOs;
using Likano.Application.Features.Manage.Product.Queries.Get;

namespace Likano.Web.Models.Manage
{
    public class EditProductViewModel
    {
        public GetProductForManageResponse Product { get; set; } = new();
        public List<CategoryDtoForManage> Categories { get; set; } = new();
        public List<BrandDtoForManage> Brands { get; set; } = new();
        public List<ProducerCountryDtoForManage> Countries { get; set; } = new();
        public List<ProductImageDto> ExistingImages { get; set; } = new();
    }

    public class ProductImageDto
    {
        public int Id { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }
    }
}