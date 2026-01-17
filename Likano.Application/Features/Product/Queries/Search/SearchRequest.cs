using Likano.Application.Common.Models;
using MediatR;

namespace Likano.Application.Features.Product.Queries.Search
{
    public class SearchRequest : IRequest<SearchResponse>
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public string? SearchString { get; set; }
        public string? Title { get; set; }
        public decimal? Price { get; set; }
        public bool? IsAvailable { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public DateTime? CreateDate { get; set; }
        public decimal? Length { get; set; } //სიგრძე
        public decimal? Width { get; set; } // სიგანე
        public decimal? Height { get; set; } // სიმაღლე
        public string? Color { get; set; }
        public int? ProducerCountryId { get; set; }
    }
}
