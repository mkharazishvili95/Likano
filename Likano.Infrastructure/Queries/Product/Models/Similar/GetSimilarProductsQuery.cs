using MediatR;

namespace Likano.Infrastructure.Queries.Product.Models.Similar
{
    public class GetSimilarProductsQuery : IRequest<GetSimilarProductsResponse>
    {
        public int ProductId { get; set; }
    }
}
