using Likano.Application.Common.Models;

namespace Likano.Application.Features.Manage.Brand.Commands.Create
{
    public class CreateBrandForManageResponse : BaseResponse
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Logo { get; set; }
    }
}