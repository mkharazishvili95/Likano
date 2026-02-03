using Likano.Domain.Enums;

namespace Likano.Web.Models.Manage
{
    public class ProductsFilterVm
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public bool? IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        public ProductStatus? Status { get; set; }
        public DateTime? CreateDateFrom { get; set; }
        public DateTime? CreateDateTo { get; set; }
        public DateTime? UpdateDateFrom { get; set; }
        public DateTime? UpdateDateTo { get; set; }
        public string? Material { get; set; }
        public decimal? LengthFrom { get; set; } //სიგრძე დან
        public decimal? LengthTo { get; set; } //სიგრძე მდე
        public decimal? WidthFrom { get; set; } // სიგანე დან
        public decimal? WidthTo { get; set; } // სიგანე მდე  
        public decimal? HeightFrom { get; set; } // სიმაღლე დან
        public decimal? HeightTo { get; set; } // სიმაღლე მდე
        public string? Color { get; set; }
        public int? ProducerCountryId { get; set; }
        public int? BrandId { get; set; }
        public string? Code { get; set; }
        public ProductType? Type { get; set; }
    }
}