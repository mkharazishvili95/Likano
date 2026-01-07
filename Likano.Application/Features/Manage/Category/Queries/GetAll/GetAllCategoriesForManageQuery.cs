using Likano.Application.Common.Models;
using MediatR;

namespace Likano.Application.Features.Manage.Category.Queries.GetAll
{
    public class GetAllCategoriesForManageQuery : IRequest<GetAllCategoriesForManageResponse>
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}
