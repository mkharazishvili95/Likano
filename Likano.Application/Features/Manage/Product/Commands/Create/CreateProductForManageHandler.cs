using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Manage.Product.Commands.Create
{
    public class CreateProductForManageHandler : IRequestHandler<CreateProductForManageCommand, CreateProductForManageResponse>
    {
        readonly IManageRepository _repository;
        public CreateProductForManageHandler(IManageRepository repository)
        {
            _repository = repository;
        }

        public async Task<CreateProductForManageResponse> Handle(CreateProductForManageCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Title))
                return new CreateProductForManageResponse { Message = "სათაური სავალდებულოა.", Success = false, StatusCode = 400 };

            var product = new Likano.Domain.Entities.Product
            {
                Title = request.Title,
                Description = request.Description,
                Price = request.Price,
                IsAvailable = true,
                ImageUrl = null,
                Material = request.Material,
                Length = request.Length,
                Width = request.Width,
                Height = request.Height,
                Color = request.Color,
                Status = Domain.Enums.ProductStatus.Active,
                CreateDate = DateTime.UtcNow.AddHours(4),
                UpdateDate = DateTime.UtcNow.AddHours(4),
                CategoryId = request.CategoryId,
                BrandId = request.BrandId,
                ProducerCountryId = request.ProducerCountryId,
                Code = request.Code,
                SeoTitle = request.SeoTitle,
                Type = request.ProductType,
                ViewCount = 0
            };

            var productId = await _repository.AddProductAsync(product);

            string? mainImageUrl = null;
            if (request.Images != null && request.Images.Count > 0)
            {
                bool hasMainSelected = request.Images.Any(x => x.IsMain);

                for (int i = 0; i < request.Images.Count; i++)
                {
                    var photo = request.Images[i];
                    bool isMain = photo.IsMain || (!hasMainSelected && i == 0);

                    var fileDto = await _repository.UploadFileAsync(
                        photo.FileName,
                        photo.FileContent,
                        Domain.Enums.File.FileType.Image,
                        null,
                        null,
                        productId,
                        null,
                        isMain
                    );

                    if (isMain)
                    {
                        mainImageUrl = fileDto.FileUrl;
                    }
                }
            }

            if (!string.IsNullOrEmpty(mainImageUrl))
            {
                product.ImageUrl = mainImageUrl;
                await _repository.UpdateProductImageUrlAsync(productId, mainImageUrl);
            }

            return new CreateProductForManageResponse
            {
                ProductId = productId,
                StatusCode = 200,
                Success = true,
                Message = "პროდუქტი წარმატებით დაემატა"
            };
        }
    }
}