using Likano.Infrastructure.Queries.Product.Models;

namespace Likano.Infrastructure.Queries.Product
{
    public static class ProductQueryFilterExtensions
    {
        public static string ToWhereClause(this GetAllProductsForSearchQuery request)
        {
            var whereConditions = new List<string> { "1=1" };

            void Add(string? condition, bool add) { if (add) whereConditions.Add(condition!); }

            Add(
                $"(p.Title LIKE N'%{request.SearchString}%' OR " +
                $"p.Description LIKE N'%{request.SearchString}%' OR " +
                $"c.Name LIKE N'%{request.SearchString}%' OR " +
                $"b.Name LIKE N'%{request.SearchString}%')",
                !string.IsNullOrEmpty(request.SearchString)
            );
            Add($"p.Title LIKE '%{request.Title}%'", !string.IsNullOrEmpty(request.Title));
            Add($"p.Code LIKE '%{request.Code}%'", !string.IsNullOrEmpty(request.Code));
            Add($"p.Price >= {request.PriceFrom}", request.PriceFrom.HasValue);
            Add($"p.Price <= {request.PriceTo}", request.PriceTo.HasValue);
            Add("p.IsAvailable = 1", request.IsAvailable == true);
            Add($"p.CategoryId = {request.CategoryId}", request.CategoryId.HasValue);
            Add($"p.BrandId = {request.BrandId}", request.BrandId.HasValue);
            Add($"p.ProducerCountryId = {request.ProducerCountryId}", request.ProducerCountryId.HasValue);
            Add($"p.Length >= {request.LengthFrom}", request.LengthFrom.HasValue);
            Add($"p.Length <= {request.LengthTo}", request.LengthTo.HasValue);
            Add(request.HasPrice == true ? "p.Price > 0" : "(p.Price IS NULL OR p.Price = 0)", request.HasPrice.HasValue);
            Add($"p.Width >= {request.WidthFrom}", request.WidthFrom.HasValue);
            Add($"p.Width <= {request.WidthTo}", request.WidthTo.HasValue);
            Add($"p.Height >= {request.HeightFrom}", request.HeightFrom.HasValue);
            Add($"p.Height <= {request.HeightTo}", request.HeightTo.HasValue);
            Add($"p.Color LIKE '%{request.Color}%'", !string.IsNullOrEmpty(request.Color));

            return string.Join(" AND ", whereConditions);
        }
    }
}