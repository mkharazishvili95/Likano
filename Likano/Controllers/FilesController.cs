using Likano.Application.Features.Manage.File.Commands.Delete;
using Likano.Application.Features.Manage.File.Commands.DeleteImage;
using Likano.Application.Features.Manage.File.Commands.Upload;
using Likano.Application.Features.Manage.File.Queries.Get;
using Likano.Application.Interfaces;
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
        readonly IManageRepository _manageRepository;
        public FilesController(IMediator mediator, IManageRepository manageRepository)
        {
            _mediator = mediator;
            _manageRepository = manageRepository;
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

        [Authorize(Roles = nameof(UserType.Admin))]
        [HttpDelete("image")]
        public async Task<IActionResult> DeleteImage([FromQuery] int? categoryId, [FromQuery] int? brandId, [FromQuery] int? productId, CancellationToken ct)
        {
            var resp = await _mediator.Send(new DeleteImageForManageCommand
            {
                CategoryId = categoryId,
                BrandId = brandId,
                ProductId = productId
            }, ct);

            if (resp.Success == true && (resp.StatusCode == null || resp.StatusCode == 204))
                return NoContent();

            if (resp.StatusCode == 404) return NotFound(resp);
            if (resp.StatusCode == 400) return BadRequest(resp);

            return StatusCode(resp.StatusCode ?? 500, resp);
        }
    }
}
