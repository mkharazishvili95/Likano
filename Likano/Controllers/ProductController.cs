using Likano.Application.Features.Product.Queries.Get;
using Likano.Application.Features.Product.Queries.GetAll;
using Likano.Infrastructure.Queries.Product.Models;
using Likano.Infrastructure.Queries.Product.Models.Details;
using Likano.Infrastructure.Queries.Product.Models.Similar;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Likano.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        readonly IMediator _mediator;
        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<GetProductResponse> Get(int id) => await _mediator.Send(new GetProductQuery { Id = id });

        [HttpPost("all")]
        public async Task<GetAllProductsResponse> GetAll(GetAllProductsQuery request) => await _mediator.Send(request);

        [HttpPost("search")]
        public async Task<GetAllProductsForSearchResponse> Search(GetAllProductsForSearchQuery request) => await _mediator.Send(request);

        [HttpGet("details/{id}")]
        public async Task<GetProductDetailsResponse> Details(int id) => await _mediator.Send(new GetProductDetailsQuery { ProductId = id });

        [HttpGet("similars")]
        public async Task<GetSimilarProductsResponse> GetSimilars(int productId) => await _mediator.Send(new GetSimilarProductsQuery { ProductId = productId });
    }
}
