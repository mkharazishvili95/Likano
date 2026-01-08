using System.Linq.Expressions;

namespace Likano.Application.Features.Manage.Brand.Queries.GetAll
{
    public static class GetAllBrandsForManageHelper
    {
        public static Expression<Func<Likano.Domain.Entities.Brand, bool>> BuildWhereClause(GetAllBrandsForManageQuery request)
        {
            Expression<Func<Likano.Domain.Entities.Brand, bool>> whereClause = p =>
                (!request.Id.HasValue || p.Id == request.Id.Value) &&
                (string.IsNullOrWhiteSpace(request.Name) || (!string.IsNullOrEmpty(p.Name) && p.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase))) &&
                (string.IsNullOrWhiteSpace(request.Description) || (!string.IsNullOrEmpty(p.Description) && p.Description.Contains(request.Description, StringComparison.OrdinalIgnoreCase))) &&
                (request.IsActive == null || p.IsActive == request.IsActive);

            return whereClause;
        }
    }
}
