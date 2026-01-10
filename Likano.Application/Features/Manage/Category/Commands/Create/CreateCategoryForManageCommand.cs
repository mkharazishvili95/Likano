using MediatR;
using Likano.Application.Common.Models;

namespace Likano.Application.Features.Manage.Category.Commands.Create
{
    public class CreateCategoryForManageCommand : IRequest<CreateCategoryForManageResponse>
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string? LogoFileName { get; set; }
        public string? LogoFileContent { get; set; }
    }
}