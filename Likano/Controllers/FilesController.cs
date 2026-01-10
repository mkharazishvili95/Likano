using Likano.Application.Features.Manage.File.Queries.Get;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Likano.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        readonly IMediator _mediator;
        public FilesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<GetFileForManageResponse> GetForManage(int id) => await _mediator.Send(new GetFileForManageQuery { FileId = id});
    }
}
