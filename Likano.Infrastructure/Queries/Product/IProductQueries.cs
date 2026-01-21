using Likano.Infrastructure.Queries.Product.Models;
using Likano.Infrastructure.Queries.Product.Models.Details;
using Likano.Infrastructure.Queries.Product.Models.Similar;

namespace Likano.Infrastructure.Queries.Product
{
    public interface IProductQueries
    {
        Task<GetAllProductsForSearchResponse> GetAllProductsForSearch(GetAllProductsForSearchQuery request);
        Task<GetProductDetailsResponse> GetProductDetails(GetProductDetailsQuery request);
        Task<GetSimilarProductsResponse> GetSimilarProducts(GetSimilarProductsQuery request);
    }
}
