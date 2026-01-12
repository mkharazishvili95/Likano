using Likano.Application.Interfaces;
using Likano.Domain.Enums.File;
using MediatR;

namespace Likano.Application.Features.Manage.File.Commands.DeleteImage
{
    public class DeleteImageForManageHandler : IRequestHandler<DeleteImageForManageCommand, DeleteImageForManageResponse>
    {
        private readonly IManageRepository _manageRepository;

        public DeleteImageForManageHandler(IManageRepository manageRepository)
        {
            _manageRepository = manageRepository;
        }

        public async Task<DeleteImageForManageResponse> Handle(DeleteImageForManageCommand request, CancellationToken cancellationToken)
        {
            var provided = new[] { request.CategoryId.HasValue, request.BrandId.HasValue, request.ProductId.HasValue }.Count(x => x);
            if (provided != 1)
                return new DeleteImageForManageResponse { Success = false, StatusCode = 400, Message = "Provide exactly one of CategoryId, BrandId or ProductId." };

            string? oldUrl = null;

            if (request.CategoryId.HasValue)
            {
                var cat = await _manageRepository.GetCategory(request.CategoryId.Value);
                if (cat is null) return new DeleteImageForManageResponse { Success = false, StatusCode = 404, Message = "Category not found." };
                oldUrl = cat.Logo;
            }
            else if (request.BrandId.HasValue)
            {
                var brand = await _manageRepository.GetBrand(request.BrandId.Value);
                if (brand is null) return new DeleteImageForManageResponse { Success = false, StatusCode = 404, Message = "Brand not found." };
                oldUrl = brand.Logo;
            }
            else if (request.ProductId.HasValue)
            {
                var product = await _manageRepository.GetProduct(request.ProductId.Value);
                if (product is null) return new DeleteImageForManageResponse { Success = false, StatusCode = 404, Message = "Product not found." };
                oldUrl = product.ImageUrl;
            }

            var cleared = await _manageRepository.DeleteImage(request.CategoryId, request.BrandId, request.ProductId);
            if (!cleared)
                return new DeleteImageForManageResponse { Success = false, StatusCode = 500, Message = "Failed to clear image on entity." };

            if (!string.IsNullOrWhiteSpace(oldUrl))
            {
                var files = await _manageRepository.GetAllFiles();
                var file = files?.FirstOrDefault(f =>
                    f.FileType == FileType.Image &&
                    ((request.CategoryId.HasValue && f.CategoryId == request.CategoryId) ||
                     (request.BrandId.HasValue && f.BrandId == request.BrandId) ||
                     (request.ProductId.HasValue && f.ProductId == request.ProductId)) &&
                    string.Equals(f.FileUrl, oldUrl, StringComparison.OrdinalIgnoreCase));

                if (file != null)
                {
                    await _manageRepository.DeleteFileAsync(file.Id);
                }
            }

            return new DeleteImageForManageResponse { Success = true, StatusCode = 204, Message = "ფოტო წარმატებით წაიშალა" };
        }
    }
}