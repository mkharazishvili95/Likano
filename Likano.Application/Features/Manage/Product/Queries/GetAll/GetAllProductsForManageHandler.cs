using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Manage.Product.Queries.GetAll
{
    public class GetAllProductsForManageHandler : IRequestHandler<GetAllProductsForManageQuery, GetAllProductsForManageResponse>
    {
        private readonly IManageRepository _manageRepository;

        public GetAllProductsForManageHandler(IManageRepository manageRepository)
        {
            _manageRepository = manageRepository;
        }

        public async Task<GetAllProductsForManageResponse> Handle(GetAllProductsForManageQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.Pagination?.PageNumber > 0 ? request.Pagination.PageNumber : 1;
            var pageSize = request.Pagination?.PageSize > 0 ? request.Pagination.PageSize : 20;

            var products = await _manageRepository.GetAllProducts() ?? new List<Domain.Entities.Product>();

            if (request.Id.HasValue)
                products = products.Where(p => p.Id == request.Id.Value).ToList();

            if (!string.IsNullOrWhiteSpace(request.Title))
                products = products.Where(p => !string.IsNullOrEmpty(p.Title) && p.Title.Contains(request.Title, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(request.Description))
                products = products.Where(p => !string.IsNullOrEmpty(p.Description) && p.Description.Contains(request.Description, StringComparison.OrdinalIgnoreCase)).ToList();

            if (request.PriceFrom.HasValue)
                products = products.Where(p => p.Price >= request.PriceFrom.Value).ToList();

            if (request.PriceTo.HasValue)
                products = products.Where(p => p.Price <= request.PriceTo.Value).ToList();

            if (request.IsAvailable.HasValue)
                products = products.Where(p => p.IsAvailable == request.IsAvailable.Value).ToList();

            if (!string.IsNullOrWhiteSpace(request.ImageUrl))
                products = products.Where(p => !string.IsNullOrEmpty(p.ImageUrl) && p.ImageUrl.Contains(request.ImageUrl, StringComparison.OrdinalIgnoreCase)).ToList();

            if (request.CategoryId.HasValue)
                products = products.Where(p => p.CategoryId == request.CategoryId.Value).ToList();

            if (request.Status.HasValue)
                products = products.Where(p => p.Status == request.Status.Value).ToList();

            if (request.CreateDateFrom.HasValue)
                products = products.Where(p => p.CreateDate >= request.CreateDateFrom.Value).ToList();

            if (request.CreateDateTo.HasValue)
                products = products.Where(p => p.CreateDate <= request.CreateDateTo.Value).ToList();

            if (request.UpdateDateFrom.HasValue)
                products = products.Where(p => p.UpdateDate.HasValue && p.UpdateDate.Value >= request.UpdateDateFrom.Value).ToList();

            if (request.UpdateDateTo.HasValue)
                products = products.Where(p => p.UpdateDate.HasValue && p.UpdateDate.Value <= request.UpdateDateTo.Value).ToList();

            var totalCount = products.Count;

            var paged = products
                .OrderByDescending(p => p.CreateDate)
                .ThenByDescending(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new GetAllProductsForManageResponse
            {
                Success = true,
                StatusCode = 200,
                TotalCount = totalCount,
                Items = paged.Select(p => new GetAllProductsForManageItemsResponse
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Price = p.Price,
                    IsAvailable = p.IsAvailable,
                    ImageUrl = p.ImageUrl,
                    CategoryId = p.CategoryId,
                    Status = p.Status,
                    CreateDate = p.CreateDate,
                    UpdateDate = p.UpdateDate
                }).ToList()
            };
        }
    }
}