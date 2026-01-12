using MediatR;

namespace Likano.Application.Features.Manage.File.Commands.DeleteImage
{
    public class DeleteImageForManageCommand : IRequest<DeleteImageForManageResponse>
    {
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public int? ProductId { get; set; }
    }
}
