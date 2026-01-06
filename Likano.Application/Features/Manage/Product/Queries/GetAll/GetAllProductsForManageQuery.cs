using Likano.Application.Common.Models;
using Likano.Domain.Enums;
using MediatR;

namespace Likano.Application.Features.Manage.Product.Queries.GetAll
{
    public class GetAllProductsForManageQuery : IRequest<GetAllProductsForManageResponse>
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public bool? IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        public ProductStatus? Status { get; set; }
        public DateTime? CreateDateFrom { get; set; }
        public DateTime? CreateDateTo { get; set; }
        public DateTime? UpdateDateFrom { get; set; }
        public DateTime? UpdateDateTo { get; set; }
    }
}
