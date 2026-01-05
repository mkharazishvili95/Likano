using MediatR;

namespace Likano.Application.Features.Product.Queries.Get
{
    public class GetProductQuery : IRequest<GetProductResponse>
    {
        public int Id { get; set; }
    }
}
