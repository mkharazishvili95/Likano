using MediatR;

namespace Likano.Application.Features.Manage.Brand.Queries.Get
{
    public class GetBrandForManageQuery : IRequest<GetBrandForManageResponse>
    {
        public int BrandId { get; set; }
    }
}
