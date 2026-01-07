using MediatR;

namespace Likano.Application.Features.Manage.Category.Commands.ChangeStatus
{
    public class ChangeActiveStatusCommand : IRequest<ChangeActiveStatusResponse>
    {
        public int CategoryId { get; set; }
    }
}
