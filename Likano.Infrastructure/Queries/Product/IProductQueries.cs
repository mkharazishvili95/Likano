using Likano.Infrastructure.Queries.Product.Models;
using Likano.Infrastructure.Queries.Product.Models.Details;

namespace Likano.Infrastructure.Queries.Product
{
    public interface IProductQueries
    {
        Task<GetAllProductsForSearchResponse> GetAllProductsForSearch(GetAllProductsForSearchQuery request);
        Task<GetProductDetailsResponse> GetProductDetails(GetProductDetailsQuery request);
    }
}
