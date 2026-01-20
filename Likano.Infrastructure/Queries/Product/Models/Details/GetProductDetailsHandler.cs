using MediatR;

namespace Likano.Infrastructure.Queries.Product.Models.Details
{
    public class GetProductDetailsHandler : IRequestHandler<GetProductDetailsQuery, GetProductDetailsResponse>
    {
        public GetProductDetailsHandler(IProductQueries productQueries)
        {
            _productQueries = productQueries;
        }
        readonly IProductQueries _productQueries;
        public async Task<GetProductDetailsResponse> Handle(GetProductDetailsQuery request, CancellationToken cancellationToken)
        {
            return await _productQueries.GetProductDetails(request);
        }
    }
}
