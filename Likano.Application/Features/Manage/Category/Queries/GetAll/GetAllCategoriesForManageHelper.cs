using Likano.Application.Features.Manage.Product.Queries.GetAll;
using System.Linq.Expressions;

namespace Likano.Application.Features.Manage.Category.Queries.GetAll
{
    public static class GetAllCategoriesForManageHelper
    {
        public static Expression<Func<Likano.Domain.Entities.Category, bool>> BuildWhereClause(GetAllCategoriesForManageQuery request)
        {
            Expression<Func<Likano.Domain.Entities.Category, bool>> whereClause = p =>
                (!request.Id.HasValue || p.Id == request.Id.Value) &&
                (string.IsNullOrWhiteSpace(request.Name) || (!string.IsNullOrEmpty(p.Name) && p.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase))) &&
                (string.IsNullOrWhiteSpace(request.Description) || (!string.IsNullOrEmpty(p.Description) && p.Description.Contains(request.Description, StringComparison.OrdinalIgnoreCase))) &&
                (string.IsNullOrWhiteSpace(request.SearchString) || (!string.IsNullOrEmpty(p.Name) && p.Name.Contains(request.SearchString, StringComparison.OrdinalIgnoreCase))) &&
                (string.IsNullOrWhiteSpace(request.SearchString) || (!string.IsNullOrEmpty(p.Description) && p.Description.Contains(request.SearchString, StringComparison.OrdinalIgnoreCase))) &&
                (!request.IsActive.HasValue || p.IsActive == request.IsActive.Value);

            return whereClause;
        }
    }
}
