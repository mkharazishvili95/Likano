using Likano.Application.Common.Models;

namespace Likano.Application.Features.Manage.Brand.Queries.Get
{
    public class GetBrandForManageResponse : BaseResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Logo { get; set; }
        public bool IsActive { get; set; }
    }
}
