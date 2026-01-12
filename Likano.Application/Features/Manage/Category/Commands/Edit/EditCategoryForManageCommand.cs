using MediatR;

namespace Likano.Application.Features.Manage.Category.Commands.Edit
{
    public class EditCategoryForManageCommand : IRequest<EditCategoryForManageResponse>
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string? LogoFileName { get; set; }
        public string? LogoFileContent { get; set; } 
        public bool RemoveLogo { get; set; } = false;
    }
}