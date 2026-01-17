using Likano.Application.Features.Brand.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Likano.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : ControllerBase
    {
        readonly IMediator _mediator;
        public BrandController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("all")]
        public async Task<GetAllBrandsResponse> GetAll(GetAllBrandsQuery request) => await _mediator.Send(request);

    }
}
