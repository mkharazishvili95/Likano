using Likano.Application.Common.Models;

namespace Likano.Application.Features.Product.Queries.Get
{
    public class GetProductResponse : BaseResponse
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
