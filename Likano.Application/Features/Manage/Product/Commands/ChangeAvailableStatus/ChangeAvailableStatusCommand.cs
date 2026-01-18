using Likano.Application.Features.Manage.Product.Commands.ChangeStatus;
using MediatR;

namespace Likano.Application.Features.Manage.Product.Commands.ChangeAvailableStatus
{
    public class ChangeAvailableStatusCommand : IRequest<ChangeAvailableStatusResponse>
    {
        public int ProductId { get; set; }
        public bool IsAvailable { get; set; }
    }
}
