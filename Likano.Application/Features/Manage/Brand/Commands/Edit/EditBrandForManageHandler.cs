using Likano.Application.Features.Manage.File.Commands.DeleteImage;
using Likano.Application.Features.Manage.File.Commands.Upload;
using Likano.Application.Interfaces;
using Likano.Domain.Enums.File;
using MediatR;

namespace Likano.Application.Features.Manage.Brand.Commands.Edit
{
    public class EditBrandForManageHandler : IRequestHandler<EditBrandForManageCommand, EditBrandForManageResponse>
    {
        readonly IManageRepository _manageRepository;
        readonly IMediator _mediator;

        public EditBrandForManageHandler(IManageRepository manageRepository, IMediator mediator)
        {
            _manageRepository = manageRepository;
            _mediator = mediator;
        }

        public async Task<EditBrandForManageResponse> Handle(EditBrandForManageCommand request, CancellationToken cancellationToken)
        {
            var updated = await _manageRepository.EditBrandAsync(request.Id, request.Name, request.Description);
            if (!updated)
            {
                return new EditBrandForManageResponse
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Brand not found or edit failed."
                };
            }

            string? logoUrl = null;

            if (request.RemoveLogo)
            {
                var del = await _mediator.Send(new DeleteImageForManageCommand { BrandId = request.Id }, cancellationToken);
                if (del.Success == false)
                {
                    return new EditBrandForManageResponse
                    {
                        Success = false,
                        StatusCode = del.StatusCode ?? 500,
                        Message = del.Message ?? "Failed to delete brand logo."
                    };
                }
            }
            else if (!string.IsNullOrWhiteSpace(request.LogoFileContent) && !string.IsNullOrWhiteSpace(request.LogoFileName))
            {
                var uploadResp = await _mediator.Send(new UploadFileForManageCommand
                {
                    FileName = request.LogoFileName!,
                    FileContent = request.LogoFileContent!,
                    FileType = FileType.Image,
                    BrandId = request.Id
                }, cancellationToken);

                if (uploadResp.Success == true && !string.IsNullOrWhiteSpace(uploadResp.FileUrl))
                {
                    logoUrl = uploadResp.FileUrl;
                    await _manageRepository.UpdateBrandLogoAsync(request.Id, logoUrl);
                }
            }

            return new EditBrandForManageResponse
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                Logo = logoUrl,
                Success = true,
                StatusCode = 200,
                Message = "ბრენდი წარმატებით დაედიტირდა"
            };
        }
    }
}