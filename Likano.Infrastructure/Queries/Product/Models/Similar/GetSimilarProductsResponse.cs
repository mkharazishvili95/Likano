using Likano.Application.Common.Models;

namespace Likano.Infrastructure.Queries.Product.Models.Similar
{
    public class GetSimilarProductsResponse : BaseResponse
    {
        public int TotalCount { get; set; }
        public List<GetSimilarProductsItemsResponse> Items { get; set; } = new();
    }
    public class GetSimilarProductsItemsResponse
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public bool? IsAvailable { get; set; }
        public string? MainImage { get; set; }
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
        public string? Code { get; set; }
        public string? SeoTitle { get; set; }
    }
}
