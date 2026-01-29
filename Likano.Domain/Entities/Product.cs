using Likano.Domain.Enums;

namespace Likano.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public bool? IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public int? BrandId { get; set; }
        public Brand? Brand { get; set; }
        public ProductStatus? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? Material { get; set; }
        public decimal? Length { get; set; } //სიგრძე
        public decimal? Width { get; set; } // სიგანე
        public decimal? Height { get; set; } // სიმაღლე
        public string? Color { get; set; }
        public ProducerCountry? ProducerCountry { get; set; } // მწარმოებელი ქვეყანა
        public int? ProducerCountryId { get; set; }
        public ICollection<Likano.Domain.Entities.File>? Images { get; set; }
        public string? Code { get; set; }
        public string? SeoTitle { get; set; }
        public int? ViewCount { get; set; }
        public ProductType? Type { get; set; }
        public string? IncludedComponents { get; set; } //კომპლექტში შედის
    }
}
