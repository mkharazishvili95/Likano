using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Manage.Product.Commands.Edit
{
    public class EditProductForManageHandler : IRequestHandler<EditProductForManageCommand, EditProductForManageResponse>
    {
        readonly IManageRepository _repository;
        public EditProductForManageHandler(IManageRepository repository)
        {
            _repository = repository;
        }

        public async Task<EditProductForManageResponse> Handle(EditProductForManageCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                return new EditProductForManageResponse { Message = "სათაური აუცილებელია.", Success = false, StatusCode = 400 };

            var product = await _repository.GetProduct(request.Id);
            if (product == null)
                return new EditProductForManageResponse { Message = "პროდუქტი ვერ მოიძებნა.", Success = false, StatusCode = 404 };

            product.Title = request.Title;
            product.Description = request.Description;
            product.Price = request.Price;
            product.CategoryId = request.CategoryId;
            product.BrandId = request.BrandId;
            product.ProducerCountryId = request.ProducerCountryId;
            product.Material = request.Material;
            product.Length = request.Length;
            product.Width = request.Width;
            product.Height = request.Height;
            product.Color = request.Color;
            product.UpdateDate = DateTime.UtcNow.AddHours(4);
            product.Code = request.Code;
            product.Type = request.ProductType;
            product.IncludedComponents = request.IncludedComponents;

            await _repository.UpdateProductAsync(product);

            if (request.DeletedImageIds != null && request.DeletedImageIds.Count > 0)
            {
                foreach (var fileId in request.DeletedImageIds)
                {
                    await _repository.DeleteFileAsync(fileId);
                }
            }

            if (request.NewImages != null && request.NewImages.Count > 0)
            {
                foreach (var photo in request.NewImages)
                {
                    await _repository.UploadFileAsync(
                        photo.FileName,
                        photo.FileContent,
                        Domain.Enums.File.FileType.Image,
                        null,
                        null,
                        request.Id,
                        null,
                        photo.IsMain
                    );
                }
            }

            if (request.MainImageId.HasValue)
            {
                await _repository.SetMainImageAsync(request.Id, request.MainImageId.Value);
            }

            var mainImage = await _repository.GetMainImageForProductAsync(request.Id);
            if (mainImage != null)
            {
                await _repository.UpdateProductImageUrlAsync(request.Id, mainImage.FileUrl);
            }
            else
            {
                await _repository.UpdateProductImageUrlAsync(request.Id, null);
            }

            return new EditProductForManageResponse
            {
                ProductId = request.Id,
                StatusCode = 200,
                Success = true,
                Message = "პროდუქტი წარმატებით განახლდა"
            };
        }
    }
}