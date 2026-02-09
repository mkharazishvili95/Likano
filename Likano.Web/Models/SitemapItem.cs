namespace Likano.Web.Models
{
    public class SitemapItem
    {
        public string Url { get; set; } = string.Empty;
        public DateTime LastModified { get; set; } = DateTime.UtcNow;
        public string ChangeFrequency { get; set; } = "weekly";
        public double Priority { get; set; } = 0.5;
    }
}
