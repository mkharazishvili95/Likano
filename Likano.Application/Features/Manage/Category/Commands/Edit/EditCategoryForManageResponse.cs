using Likano.Application.Common.Models;

namespace Likano.Application.Features.Manage.Category.Commands.Edit
{
    public class EditCategoryForManageResponse : BaseResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string? Logo { get; set; }
    }
}