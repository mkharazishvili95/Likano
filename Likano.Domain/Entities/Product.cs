using Likano.Domain.Enums;

namespace Likano.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public bool? IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public ProductStatus? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
