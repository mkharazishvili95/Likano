using Likano.Application.Common.Models;
using MediatR;

namespace Likano.Application.Features.Brand.Queries.GetAll
{
    public class GetAllBrandsQuery : IRequest<GetAllBrandsResponse>
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public string? SearchString { get; set; }
    }
}
