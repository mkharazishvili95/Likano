using Likano.Application.Features.Manage.File.Commands.Delete;
using Likano.Application.Features.Manage.File.Commands.Upload;
using Likano.Application.Features.Manage.File.Queries.Get;
using Likano.Domain.Enums.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = nameof(UserType.Admin))]
        [HttpGet("{id}")]
        public async Task<GetFileForManageResponse> GetForManage(int id) => await _mediator.Send(new GetFileForManageQuery { FileId = id});

        [Authorize(Roles = nameof(UserType.Admin))]
        [HttpDelete("{id}")]
        public async Task<DeleteFileForManageResponse> DeleteForManage(int id) => await _mediator.Send(new DeleteFileForManageCommand { FileId = id });

        [Authorize(Roles = nameof(UserType.Admin))]
        [HttpPost("upload")]
        public async Task<UploadFileForManageResponse> UploadForManage([FromBody] UploadFileForManageCommand request) => await _mediator.Send(request);
    }
}
