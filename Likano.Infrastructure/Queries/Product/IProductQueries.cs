using Likano.Infrastructure.Queries.Product.Models;

namespace Likano.Infrastructure.Queries.Product
{
    public interface IProductQueries
    {
        Task<GetAllProductsForSearchResponse> GetAllProductsForSearch(GetAllProductsForSearchQuery request);
    }
}
