using Likano.Application.Common.Models;
using Likano.Domain.Enums;

namespace Likano.Application.Features.Manage.Product.Queries.GetAll
{
    public class GetAllProductsForManageResponse : BaseResponse
    {
        public int TotalCount { get; set; }
        public List<GetAllProductsForManageItemsResponse> Items { get; set; } = new();
    }
    public class GetAllProductsForManageItemsResponse
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public bool? IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        public ProductStatus? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? Material { get; set; }
        public decimal? Length { get; set; } //სიგრძე
        public decimal? Width { get; set; } // სიგანე
        public decimal? Height { get; set; } // სიმაღლე
        public string? Color { get; set; }
        public int? ProducerCountryId { get; set; }
        public int? BrandId { get; set; }
        public string? Code { get; set; }
        public ProductType? Type { get; set; }
    }
}
