using MediatR;

namespace Likano.Application.Features.Manage.Product.Queries.Get
{
    public class GetProductForManageQuery : IRequest<GetProductForManageResponse>
    {
        public int Id { get; set; }
    }
}
