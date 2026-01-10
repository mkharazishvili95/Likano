using Likano.Domain.Enums.File;

namespace Likano.Application.DTOs
{
    public class FileDto
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public FileType? FileType { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public int? ProductId { get; set; }
    }
}
