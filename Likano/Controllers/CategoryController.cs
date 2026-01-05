using Likano.Application.Features.Category.Queries.Get;
using Likano.Application.Features.Category.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Likano.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        readonly IMediator _mediator;
        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<GetCategoryResponse> Get(int id) => await _mediator.Send(new GetCategoryQuery { Id = id });

        [HttpPost("all")]
        public async Task<GetAllCategoriesResponse> GetAll(GetAllCategoriesQuery request) => await _mediator.Send(request);
    }
}
