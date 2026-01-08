using Likano.Application.Common.Models;

namespace Likano.Application.Features.Manage.Brand.Queries.GetAll
{
    public class GetAllBrandsForManageResponse : BaseResponse
    {
        public int TotalCount { get; set; }
        public List<GetAllBrandsForManageItemsResponse> Items { get; set; } = new();
    }
    public class GetAllBrandsForManageItemsResponse
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Logo { get; set; }
        public bool? IsActive { get; set; }
    }
}
