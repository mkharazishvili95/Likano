using System.Linq.Expressions;

namespace Likano.Application.Features.Manage.Product.Queries.GetAll
{
    public static class GetAllProductsForManageHelper
    {
        public static Expression<Func<Likano.Domain.Entities.Product, bool>> BuildWhereClause(GetAllProductsForManageQuery request)
        {
            Expression<Func<Likano.Domain.Entities.Product, bool>> whereClause = p =>
                (!request.Id.HasValue || p.Id == request.Id.Value) &&
                (string.IsNullOrWhiteSpace(request.Title) || (!string.IsNullOrEmpty(p.Title) && p.Title.Contains(request.Title, StringComparison.OrdinalIgnoreCase))) &&
                (string.IsNullOrWhiteSpace(request.Description) || (!string.IsNullOrEmpty(p.Description) && p.Description.Contains(request.Description, StringComparison.OrdinalIgnoreCase))) &&
                (!request.PriceFrom.HasValue || (p.Price.HasValue && p.Price.Value >= request.PriceFrom.Value)) &&
                (!request.PriceTo.HasValue || (p.Price.HasValue && p.Price.Value <= request.PriceTo.Value)) &&
                (!request.IsAvailable.HasValue || p.IsAvailable == request.IsAvailable.Value) &&
                (string.IsNullOrWhiteSpace(request.ImageUrl) || (!string.IsNullOrEmpty(p.ImageUrl) && p.ImageUrl.Contains(request.ImageUrl, StringComparison.OrdinalIgnoreCase))) &&
                (!request.CategoryId.HasValue || p.CategoryId == request.CategoryId.Value) &&
                (!request.Status.HasValue || p.Status == request.Status.Value) &&
                (!request.CreateDateFrom.HasValue || (p.CreateDate.HasValue && p.CreateDate.Value >= request.CreateDateFrom.Value)) &&
                (!request.CreateDateTo.HasValue || (p.CreateDate.HasValue && p.CreateDate.Value <= request.CreateDateTo.Value)) &&
                (!request.UpdateDateFrom.HasValue || (p.UpdateDate.HasValue && p.UpdateDate.Value >= request.UpdateDateFrom.Value)) &&
                (!request.UpdateDateTo.HasValue || (p.UpdateDate.HasValue && p.UpdateDate.Value <= request.UpdateDateTo.Value));

            return whereClause;
        }
    }
}
