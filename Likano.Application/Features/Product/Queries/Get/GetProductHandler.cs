using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Product.Queries.Get
{
    public class GetProductHandler : IRequestHandler<GetProductQuery, GetProductResponse>
    {
        readonly IProductRepository _repository;
        public GetProductHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetProductResponse> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
                return new GetProductResponse { Description = "request required.", Success = false, StatusCode = 400 };

            var product = await _repository.Get(request.Id);

            if (product == null)
                return new GetProductResponse { Description = "Product not found.", Success = false, StatusCode = 404 };

            return new GetProductResponse
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                Title = product.Title,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                IsAvailable = product.IsAvailable,
                Description = product.Description,
                CreateDate = product.CreateDate,
                UpdateDate = product.UpdateDate,
                Success = true,
                StatusCode = 200
            };
        }
    }
}
