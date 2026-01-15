namespace Likano.Application.DTOs
{
    public class PhotoUploadDto
    {
        public string FileName { get; set; } = default!;
        public string FileContent { get; set; } = default!; 
        public bool IsMain { get; set; }
    }
}
