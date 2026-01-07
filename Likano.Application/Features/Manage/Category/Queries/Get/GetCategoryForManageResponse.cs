using Likano.Application.Common.Models;

namespace Likano.Application.Features.Manage.Category.Queries.Get
{
    public class GetCategoryForManageResponse : BaseResponse
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Logo { get; set; }
        public bool? IsActive { get; set; }
    }
}
