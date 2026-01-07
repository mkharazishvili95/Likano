using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Manage.Product.Queries.Get
{
    public class GetProductForManageHandler : IRequestHandler<GetProductForManageQuery, GetProductForManageResponse>
    {
        readonly IManageRepository _repository;
        public GetProductForManageHandler(IManageRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetProductForManageResponse> Handle(GetProductForManageQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
                return new GetProductForManageResponse { Description = "request required.", Success = false, StatusCode = 400 };

            var product = await _repository.GetProduct(request.Id);

            if (product == null)
                return new GetProductForManageResponse { Description = "Product not found.", Success = false, StatusCode = 404 };

            return new GetProductForManageResponse
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                CategoryTitle = product.Category?.Name,
                Title = product.Title,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                IsAvailable = product.IsAvailable,
                Description = product.Description,
                CreateDate = product.CreateDate,
                UpdateDate = product.UpdateDate,
                Status = product.Status,
                Success = true,
                StatusCode = 200
            };
        }
    }
}
