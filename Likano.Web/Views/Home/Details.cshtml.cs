using Likano.Infrastructure.Queries.Product;
using Likano.Infrastructure.Queries.Product.Models.Details;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class DetailsModel : PageModel
{
    readonly IProductQueries _productQueries;

    public GetProductDetailsResponse Product { get; set; }

    public DetailsModel(IProductQueries productQueries)
    {
        _productQueries = productQueries;
    }

    public async Task<IActionResult> OnGetAsync(string seoTitle, int id)
    {
        var query = new GetProductDetailsQuery { ProductId = id };
        Product = await _productQueries.GetProductDetails(query);
        if (Product == null || Product.Id == null)
        {
            return NotFound();
        }
        return Page();
    }
}