using Likano.Application.Common.Models;
using MediatR;

namespace Likano.Application.Features.Category.Queries.GetAll
{
    public class GetAllCategoriesQuery : IRequest<GetAllCategoriesResponse>
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public string? SearchString { get; set; }
    }
}