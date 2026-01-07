using Likano.Application.Common.Models;

namespace Likano.Application.Features.Manage.Category.Queries.GetAll
{
    public class GetAllCategoriesForManageResponse : BaseResponse
    {
        public int TotalCount { get; set; }
        public List<GetAllCategoriesForManageItemsResponse> Items { get; set; } = new();
    }
    public class GetAllCategoriesForManageItemsResponse
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Logo { get; set; }
        public bool? IsActive { get; set; }
    }
}
