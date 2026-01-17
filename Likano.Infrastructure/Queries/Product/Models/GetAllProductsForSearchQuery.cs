using Likano.Application.Common.Models;
using MediatR;

namespace Likano.Infrastructure.Queries.Product.Models
{
    public class GetAllProductsForSearchQuery : IRequest<GetAllProductsForSearchResponse>
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public string? SearchString { get; set; }
        public string? Title { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public bool? IsAvailable { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public int? ProducerCountryId { get; set; }
        public decimal? LengthFrom { get; set; } //სიგრძე(დან)
        public decimal? LengthTo { get; set; } //სიგრძე(მდე)
        public decimal? WidthFrom { get; set; } // სიგანე(დან)
        public decimal? WidthTo { get; set; } // სიგანე(მდე)
        public decimal? HeightFrom { get; set; } // სიმაღლე(დან)
        public decimal? HeightTo { get; set; } // სიმაღლე(მდე)
        public string? Color { get; set; }
        public SortBy? SortBy { get; set; }
    }
    public enum SortBy
    {
        PriceAsc,
        PriceDesc
    }
}
