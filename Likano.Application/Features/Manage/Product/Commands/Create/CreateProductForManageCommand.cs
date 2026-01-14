using MediatR;
using Microsoft.AspNetCore.Http;

namespace Likano.Application.Features.Manage.Product.Commands.Create
{
    public class CreateProductForManageCommand : IRequest<CreateProductForManageResponse>
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public bool? IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public int? BrandId { get; set; }
        public int? ProducerCountryId { get; set; }
        public string? Material { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public string? Color { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}