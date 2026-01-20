using MediatR;

namespace Likano.Infrastructure.Queries.Product.Models.Details
{
    public class GetProductDetailsQuery : IRequest<GetProductDetailsResponse>
    {
        public int ProductId { get; set; }  
    }
}
