namespace Likano.Application.Common.Models
{
    public class BaseResponse
    {
        public int? StatusCode { get; set; }
        public bool? Success { get; set; }
        public string? Message { get; set; }
    }
}
