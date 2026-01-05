using Likano.Application.Common.Models;

namespace Likano.Application.Features.Product.Queries.GetAll
{
    public class GetAllProductsResponse : BaseResponse
    {
        public int TotalCount { get; set; }
        public List<GetAllProductsItemsResponse> Items { get; set; } = new();
    }

    public class GetAllProductsItemsResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public bool? IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
    }
}