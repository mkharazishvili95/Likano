using Likano.Application.Common.Models;
using Likano.Domain.Enums;

namespace Likano.Application.Features.Manage.Product.Queries.Get
{
    public class GetProductForManageResponse : BaseResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public bool? IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryTitle { get; set; }
        public int? BrandId { get; set; }
        public string? BrandTitle { get; set; }
        public ProductStatus? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? Material { get; set; }
        public decimal? Length { get; set; } //სიგრძე
        public decimal? Width { get; set; } // სიგანე
        public decimal? Height { get; set; } // სიმაღლე
        public string? Color { get; set; }
        public int? ProducerCountryId { get; set; }
        public string? ProducerCountryName { get; set; }
        public string? Code { get; set; }
        public ProductType? ProductType { get; set; }
        public string? IncludedComponents { get; set; }
    }
}
