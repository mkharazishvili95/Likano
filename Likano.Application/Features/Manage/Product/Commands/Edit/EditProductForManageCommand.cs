using Likano.Application.DTOs;
using Likano.Domain.Enums;
using MediatR;

namespace Likano.Application.Features.Manage.Product.Commands.Edit
{
    public class EditProductForManageCommand : IRequest<EditProductForManageResponse>
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int CategoryId { get; set; }
        public int? BrandId { get; set; }
        public int? ProducerCountryId { get; set; }
        public string? Material { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public string? Color { get; set; }
        public List<PhotoUploadDto>? NewImages { get; set; }
        public int? MainImageId { get; set; }
        public List<int>? DeletedImageIds { get; set; }
        public string? Code { get; set; }
        public string? SeoTitle { get; set; }
        public ProductType? ProductType { get; set; }
        public string? IncludedComponents { get; set; }
    }
}