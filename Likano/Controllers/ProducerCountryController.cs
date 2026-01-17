using Likano.Application.Features.ProducerCountry.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Likano.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProducerCountryController : ControllerBase
    {
        readonly IMediator _mediator;
        public ProducerCountryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("all")]
        public async Task<GetAllProducerCountriesResponse> GetAll(GetAllProducerCountriesQuery request) => await _mediator.Send(request);
    }
}
