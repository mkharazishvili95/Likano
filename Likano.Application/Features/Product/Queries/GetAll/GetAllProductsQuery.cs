using Likano.Application.Common.Models;
using MediatR;

namespace Likano.Application.Features.Product.Queries.GetAll
{
    public class GetAllProductsQuery : IRequest<GetAllProductsResponse>
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public string? SearchString { get; set; }
        public int? CategoryId { get; set; }
    }
}