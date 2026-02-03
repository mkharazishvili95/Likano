using Likano.Application.Interfaces;
using MediatR;
using System.Linq.Expressions;

namespace Likano.Application.Features.Manage.Product.Queries.GetAll
{
    public class GetAllProductsForManageHandler : IRequestHandler<GetAllProductsForManageQuery, GetAllProductsForManageResponse>
    {
        readonly IManageRepository _manageRepository;

        public GetAllProductsForManageHandler(IManageRepository manageRepository)
        {
            _manageRepository = manageRepository;
        }

        public async Task<GetAllProductsForManageResponse> Handle(GetAllProductsForManageQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.Pagination?.PageNumber > 0 ? request.Pagination.PageNumber : 1;
            var pageSize = request.Pagination?.PageSize > 0 ? request.Pagination.PageSize : 20;

            var products = await _manageRepository.GetAllProducts() ?? new List<Domain.Entities.Product>();

            Expression<Func<Likano.Domain.Entities.Product, bool>> predicate = GetAllProductsForManageHelper.BuildWhereClause(request);
            var filtered = products.AsQueryable().Where(predicate).ToList();

            var totalCount = filtered.Count;

            var paged = filtered
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
                    UpdateDate = p.UpdateDate,
                    Material = p.Material,
                    Length = p.Length,
                    Width = p.Width,
                    Height = p.Height,
                    Color = p.Color,
                    Code = p.Code,
                    ProducerCountryId = p.ProducerCountryId,
                    BrandId = p.BrandId,
                    Type = p.Type
                }).ToList()
            };
        }
    }
}