using Likano.Domain.Enums;
using MediatR;

namespace Likano.Application.Features.Manage.Product.Commands.ChangeStatus
{
    public class ChangeProductStatusCommand : IRequest<ChangeProductStatusResponse>
    {
        public int ProductId { get; set; }
        public ProductStatus Status { get; set; }
    }
}
