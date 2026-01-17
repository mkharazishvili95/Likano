using Likano.Application.Common.Models;

namespace Likano.Application.Features.Brand.Queries.GetAll
{
    public class GetAllBrandsResponse : BaseResponse
    {
        public int TotalCount { get; set; }
        public List<GetAllBrandsItemsResponse> Items { get; set; } = new();
    }
    public class GetAllBrandsItemsResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Logo { get; set; }
    }
}
