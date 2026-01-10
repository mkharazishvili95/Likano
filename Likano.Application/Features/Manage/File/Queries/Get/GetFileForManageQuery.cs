using MediatR;

namespace Likano.Application.Features.Manage.File.Queries.Get
{
    public class GetFileForManageQuery : IRequest<GetFileForManageResponse>
    {
        public int FileId { get; set; }
    }
}
