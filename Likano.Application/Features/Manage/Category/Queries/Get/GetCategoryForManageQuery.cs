using MediatR;

namespace Likano.Application.Features.Manage.Category.Queries.Get
{
    public class GetCategoryForManageQuery : IRequest<GetCategoryForManageResponse>
    {
        public int CategoryId { get; set; }
    }
}
