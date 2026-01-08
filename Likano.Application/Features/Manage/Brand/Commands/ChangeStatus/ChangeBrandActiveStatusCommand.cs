using MediatR;

namespace Likano.Application.Features.Manage.Brand.Commands.ChangeStatus
{
    public class ChangeBrandActiveStatusCommand : IRequest<ChangeBrandActiveStatusResponse>
    {
        public int BrandId { get; set; }
    }
}
