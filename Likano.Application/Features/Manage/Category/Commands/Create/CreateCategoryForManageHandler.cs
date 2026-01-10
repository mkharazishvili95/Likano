using Likano.Application.Interfaces;
using Likano.Application.Features.Manage.File.Commands.Upload;
using Likano.Domain.Entities;
using MediatR;
using Likano.Domain.Enums.File;

namespace Likano.Application.Features.Manage.Category.Commands.Create
{
    public class CreateCategoryForManageHandler : IRequestHandler<CreateCategoryForManageCommand, CreateCategoryForManageResponse>
    {
        readonly IManageRepository _manageRepository;
        readonly IMediator _mediator;

        public CreateCategoryForManageHandler(IManageRepository manageRepository, IMediator mediator)
        {
            _manageRepository = manageRepository;
            _mediator = mediator;
        }

        public async Task<CreateCategoryForManageResponse> Handle(CreateCategoryForManageCommand request, CancellationToken cancellationToken)
        {
            var createdCategoryId = await _manageRepository.AddCategoryAsync(new Likano.Domain.Entities.Category
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = true
            });

            if (createdCategoryId <= 0)
            {
                return new CreateCategoryForManageResponse
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "Failed to create category."
                };
            }

            string? logoUrl = null;

            if (!string.IsNullOrWhiteSpace(request.LogoFileContent) && !string.IsNullOrWhiteSpace(request.LogoFileName))
            {
                var uploadResp = await _mediator.Send(new UploadFileForManageCommand
                {
                    FileName = request.LogoFileName,
                    FileContent = request.LogoFileContent,
                    FileType = FileType.Image,
                    CategoryId = createdCategoryId
                }, cancellationToken);

                if (uploadResp.Success.HasValue && uploadResp.Success.Value == true && !string.IsNullOrWhiteSpace(uploadResp.FileUrl))
                {
                    logoUrl = uploadResp.FileUrl;
                    await _manageRepository.UpdateCategoryLogoAsync(createdCategoryId, logoUrl);
                }
            }

            return new CreateCategoryForManageResponse
            {
                Id = createdCategoryId,
                Name = request.Name,
                Description = request.Description,
                Logo = logoUrl,
                Success = true,
                StatusCode = 201,
                Message = logoUrl is null ? "Category created." : "Category created with logo."
            };
        }
    }
}