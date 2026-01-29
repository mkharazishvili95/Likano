using Likano.Application.Common.Models;
using Likano.Domain.Enums;

namespace Likano.Infrastructure.Queries.Product.Models.Details
{
    public class GetProductDetailsResponse : BaseResponse
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public bool? IsAvailable { get; set; }
        public string? ProductImageUrl { get; set; }
        public DateTime? CreateDate { get; set; }
        public decimal? Length { get; set; } //სიგრძე
        public decimal? Width { get; set; } // სიგანე
        public decimal? Height { get; set; } // სიმაღლე
        public string? Color { get; set; }
        public int? CategoryId { get; set; }
        public CategoryDtoForSearch? Category { get; set; }
        public int? ProducerCountryId { get; set; }
        public ProducerCountryDtoForSearch? ProducerCountry { get; set; }
        public int? BrandId { get; set; }
        public BrandDtoForSearch? Brand { get; set; }
        public List<ImageDtoForSearch>? Images { get; set; }
        public string? Code { get; set; }
        public int? ViewCount { get; set; }
        public ProductType? Type { get; set; }
        public string? IncludedComponents { get; set; }
    }
}
