using MediatR;

namespace Likano.Application.Features.Manage.Brand.Commands.Change
{
    public class ChangeBrandCommand : IRequest<ChangeBrandResponse>
    {
        public int ProductId { get; set; }
        public int NewBrandId { get; set; }
    }
}
