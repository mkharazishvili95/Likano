using MediatR;

namespace Likano.Infrastructure.Queries.Product.Models
{
    public class GetAllProductsForSearchHandler : IRequestHandler<GetAllProductsForSearchQuery, GetAllProductsForSearchResponse>
    {
        public GetAllProductsForSearchHandler(IProductQueries productQueries)
        {
            _productQueries = productQueries;
        }
        readonly IProductQueries _productQueries;
        public async Task<GetAllProductsForSearchResponse> Handle(GetAllProductsForSearchQuery request, CancellationToken cancellationToken)
        {
            return await _productQueries.GetAllProductsForSearch(request);
        }
    }
}
