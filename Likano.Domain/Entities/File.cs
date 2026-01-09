using Likano.Domain.Enums.File;

namespace Likano.Domain.Entities
{
    public class File
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public FileType? FileType { get; set; }
        public DateTime? UploadDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeleteDate { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
        public int? BrandId { get; set; }
        public Brand? Brand { get; set; }
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public bool? MainImage { get; set; }
    }
}
