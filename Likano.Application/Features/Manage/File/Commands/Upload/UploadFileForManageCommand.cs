using Likano.Domain.Enums.File;
using MediatR;

namespace Likano.Application.Features.Manage.File.Commands.Upload
{
    public class UploadFileForManageCommand : IRequest<UploadFileForManageResponse>
    {
        public string? FileName { get; set; }
        public string? FileContent { get; set; }
        public FileType? FileType { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public int? ProductId { get; set; }
        public int? UserId { get; set; }
    }
}
