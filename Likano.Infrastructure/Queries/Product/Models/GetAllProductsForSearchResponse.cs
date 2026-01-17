using Likano.Domain.Entities;
using Likano.Domain.Enums.File;

namespace Likano.Infrastructure.Queries.Product.Models
{
    public class GetAllProductsForSearchResponse
    {
        public int TotalCount { get; set; }
        public List<GetAllProductsForSearchItemsResponse> Items { get; set; } = new();
    }
    public class GetAllProductsForSearchItemsResponse
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
    }

    public class BrandDtoForSearch
    {
        public int? BrandId { get; set; }
        public string? Name { get; set; }
        public string? Logo { get; set; }
    }
    public class ProducerCountryDtoForSearch
    {
        public int? ProducerCountryId { get; set; }
        public string? Name { get; set; }
    }

    public class CategoryDtoForSearch
    {
        public int? CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Logo { get; set; }
    }
    public class ImageDtoForSearch
    {
        public int ImageId { get; set; }
        public int? ProductId { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public bool? MainImage { get; set; }
    }
}