using MediatR;

namespace Likano.Application.Features.Manage.File.Commands.Delete
{
    public class DeleteFileForManageCommand : IRequest<DeleteFileForManageResponse>
    {
        public int FileId { get; set; }
    }
}
