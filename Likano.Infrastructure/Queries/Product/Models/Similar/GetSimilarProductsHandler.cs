using MediatR;

namespace Likano.Infrastructure.Queries.Product.Models.Similar
{
    public class GetSimilarProductsHandler : IRequestHandler<GetSimilarProductsQuery, GetSimilarProductsResponse>
    {
        public GetSimilarProductsHandler(IProductQueries productQueries)
        {
            _productQueries = productQueries;
        }
        readonly IProductQueries _productQueries;
        public async Task<GetSimilarProductsResponse> Handle(GetSimilarProductsQuery request, CancellationToken cancellationToken)
        {
            return await _productQueries.GetSimilarProducts(request);
        }
    }
}
