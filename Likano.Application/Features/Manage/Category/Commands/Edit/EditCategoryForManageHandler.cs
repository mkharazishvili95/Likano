using Likano.Application.Features.Manage.File.Commands.Upload;
using Likano.Application.Interfaces;
using Likano.Domain.Enums.File;
using MediatR;

namespace Likano.Application.Features.Manage.Category.Commands.Edit
{
    public class EditCategoryForManageHandler : IRequestHandler<EditCategoryForManageCommand, EditCategoryForManageResponse>
    {
        readonly IManageRepository _manageRepository;
        readonly IMediator _mediator;

        public EditCategoryForManageHandler(IManageRepository manageRepository, IMediator mediator)
        {
            _manageRepository = manageRepository;
            _mediator = mediator;
        }

        public async Task<EditCategoryForManageResponse> Handle(EditCategoryForManageCommand request, CancellationToken cancellationToken)
        {
            var updated = await _manageRepository.EditCategoryAsync(request.Id, request.Name, request.Description);
            if (!updated)
            {
                return new EditCategoryForManageResponse
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Category not found or edit failed."
                };
            }

            string? logoUrl = null;

            if (request.RemoveLogo)
            {
                var existing = await _manageRepository.GetCategory(request.Id);
                var oldLogo = existing?.Logo;

                var cleared = await _manageRepository.DeleteImage(request.Id, null, null);
                if (!cleared)
                {
                    return new EditCategoryForManageResponse
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "Category not found or edit failed."
                    };
                }

                if (!string.IsNullOrWhiteSpace(oldLogo))
                {
                    var files = await _manageRepository.GetAllFiles();
                    var categoryLogo = files?.FirstOrDefault(f =>
                        f.CategoryId == request.Id &&
                        f.FileType == FileType.Image &&
                        string.Equals(f.FileUrl, oldLogo, StringComparison.OrdinalIgnoreCase));

                    if (categoryLogo != null)
                    {
                        await _manageRepository.DeleteFileAsync(categoryLogo.Id);
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(request.LogoFileContent) && !string.IsNullOrWhiteSpace(request.LogoFileName))
            {
                var uploadResp = await _mediator.Send(new UploadFileForManageCommand
                {
                    FileName = request.LogoFileName!,
                    FileContent = request.LogoFileContent!,
                    FileType = FileType.Image,
                    CategoryId = request.Id
                }, cancellationToken);

                if (uploadResp.Success == true && !string.IsNullOrWhiteSpace(uploadResp.FileUrl))
                {
                    logoUrl = uploadResp.FileUrl;
                    await _manageRepository.UpdateCategoryLogoAsync(request.Id, logoUrl);
                }
            }

            return new EditCategoryForManageResponse
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                Logo = logoUrl,
                Success = true,
                StatusCode = 200,
                Message = "კატეგორია წარმატებით დარედაქტირდა"
            };
        }
    }
}