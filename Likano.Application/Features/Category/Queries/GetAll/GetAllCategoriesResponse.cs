using Likano.Application.Common.Models;

namespace Likano.Application.Features.Category.Queries.GetAll
{
    public class GetAllCategoriesResponse : BaseResponse
    {
        public int TotalCount { get; set; }
        public List<GetAllCategoriesItemsResponse> Items { get; set; } = new();
    }
    public class GetAllCategoriesItemsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Logo { get; set; }
    }
}