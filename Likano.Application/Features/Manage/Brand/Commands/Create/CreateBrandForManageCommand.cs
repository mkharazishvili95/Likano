using MediatR;

namespace Likano.Application.Features.Manage.Brand.Commands.Create
{
    public class CreateBrandForManageCommand : IRequest<CreateBrandForManageResponse>
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string? LogoFileName { get; set; }
        public string? LogoFileContent { get; set; }
    }
}