using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Product.Queries.GetAll
{
    public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, GetAllProductsResponse>
    {
        private readonly IProductRepository _repository;

        public GetAllProductsHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetAllProductsResponse> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.Pagination?.PageNumber > 0 ? request.Pagination.PageNumber : 1;
            var pageSize = request.Pagination?.PageSize > 0 ? request.Pagination.PageSize : 20;
            var search = request.SearchString?.Trim();
            var categoryId = request.CategoryId;

            var products = await _repository.GetAll() ?? new List<Domain.Entities.Product>();

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value).ToList();
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                products = products
                    .Where(p =>
                        (!string.IsNullOrEmpty(p.Title) && p.Title.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(p.Description) && p.Description.Contains(search, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            var totalCount = products.Count;

            var paged = products
                .OrderBy(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new GetAllProductsResponse
            {
                Success = true,
                StatusCode = 200,
                TotalCount = totalCount,
                Items = paged.Select(p => new GetAllProductsItemsResponse
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Price = p.Price,
                    IsAvailable = p.IsAvailable,
                    ImageUrl = p.ImageUrl,
                    CategoryId = p.CategoryId,
                    CreateDate = p.CreateDate,
                    UpdateDate = p.UpdateDate
                }).ToList()
            };
        }
    }
}