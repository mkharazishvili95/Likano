using Likano.Application.Features.Manage.File.Commands.Upload;
using Likano.Application.Interfaces;
using Likano.Domain.Entities;
using Likano.Domain.Enums.File;
using MediatR;

namespace Likano.Application.Features.Manage.Brand.Commands.Create
{
    public class CreateBrandForManageHandler : IRequestHandler<CreateBrandForManageCommand, CreateBrandForManageResponse>
    {
        readonly IManageRepository _manageRepository;
        readonly IMediator _mediator;

        public CreateBrandForManageHandler(IManageRepository manageRepository, IMediator mediator)
        {
            _manageRepository = manageRepository;
            _mediator = mediator;
        }

        public async Task<CreateBrandForManageResponse> Handle(CreateBrandForManageCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return new CreateBrandForManageResponse { Success = false, StatusCode = 400, Message = "Name is required." };

            var brand = new Likano.Domain.Entities.Brand { Name = request.Name.Trim(), Description = request.Description, IsActive = true };
            var id = await _manageRepository.AddBrandAsync(brand);

            string? logoUrl = null;
            if (!string.IsNullOrWhiteSpace(request.LogoFileContent) && !string.IsNullOrWhiteSpace(request.LogoFileName))
            {
                var uploadResp = await _mediator.Send(new UploadFileForManageCommand
                {
                    FileName = request.LogoFileName!,
                    FileContent = request.LogoFileContent!,
                    FileType = FileType.Image,
                    BrandId = id
                }, cancellationToken);

                if (uploadResp.Success == true && !string.IsNullOrWhiteSpace(uploadResp.FileUrl))
                {
                    logoUrl = uploadResp.FileUrl;
                    await _manageRepository.UpdateBrandLogoAsync(id, logoUrl);
                }
            }

            return new CreateBrandForManageResponse
            {
                Id = id,
                Name = brand.Name,
                Description = brand.Description,
                Logo = logoUrl,
                Success = true,
                StatusCode = 200,
                Message = "ბრენდი წარმატებით შეიქმნა"
            };
        }
    }
}