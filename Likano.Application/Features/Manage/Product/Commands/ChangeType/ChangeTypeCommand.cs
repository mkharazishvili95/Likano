using Likano.Domain.Enums;
using MediatR;

namespace Likano.Application.Features.Manage.Product.Commands.ChangeType
{
    public class ChangeTypeCommand : IRequest<ChangeTypeResponse>
    {
        public int ProductId { get; set; }
        public ProductType NewType { get; set; }
    }
}
