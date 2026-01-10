using Likano.Application.Common.Models;
using Likano.Domain.Enums.File;

namespace Likano.Application.Features.Manage.File.Commands.Upload
{
    public class UploadFileForManageResponse : BaseResponse
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public FileType? FileType { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public int? ProductId { get; set; }
    }
}
