using Imagekit.Sdk;
using Likano.Application.Configuration;
using Likano.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Likano.Application.Features.Manage.File.Commands.Upload
{
    public class UploadFileForManageHandler : IRequestHandler<UploadFileForManageCommand, UploadFileForManageResponse>
    {
        readonly ImagekitClient _imageKitClient;
        readonly IMediator _mediator;
        readonly IHttpContextAccessor _httpContextAccessor;
        readonly IManageRepository _manageRepository;
        readonly IUserRepository _userRepository;
        public UploadFileForManageHandler(
            IOptions<ImageKitSettings> imageKitOptions,
            IMediator mediator,
            IHttpContextAccessor httpContextAccessor,
            IManageRepository manageRepository,
            IUserRepository userRepository)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
            _manageRepository = manageRepository;
            _userRepository = userRepository;

            var settings = imageKitOptions.Value;
            _imageKitClient = new ImagekitClient( settings.PublicKey, settings.PrivateKey, settings.UrlEndpoint);
        }

        public async Task<UploadFileForManageResponse> Handle(UploadFileForManageCommand request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return new UploadFileForManageResponse { Success = false, StatusCode = 401, Message = "User is not authenticated." };

            var user = await _userRepository.GetByIdAsync(int.Parse(userIdClaim));

            if (user == null)
                return new UploadFileForManageResponse { Success = false, StatusCode = 401, Message = "User is not authenticated." };

            if (string.IsNullOrWhiteSpace(request.FileContent) || string.IsNullOrWhiteSpace(request.FileName))
                throw new ArgumentException("FileName and FileContent must be provided");

            string base64Data = request.FileContent;
            string extension = ".bin";

            if (request.FileContent.StartsWith("data:"))
            {
                var parts = request.FileContent.Split(',', 2);
                if (parts.Length == 2)
                {
                    var mime = parts[0].Split(':', ';')[1];
                    extension = mime switch
                    {
                        "image/jpeg" => ".jpg",
                        "image/png" => ".png",
                        "video/mp4" => ".mp4",
                        "application/pdf" => ".pdf",
                        _ => ".bin"
                    };
                    base64Data = parts[1];
                }
            }

            string uniqueFileName = $"{Guid.NewGuid()}{extension}";

            var uploadRequest = new FileCreateRequest
            {
                fileName = uniqueFileName,
                folder = "/gifts",
                useUniqueFileName = true,
                file = base64Data
            };

            dynamic uploadResult;
            try
            {
                uploadResult = await _imageKitClient.UploadAsync(uploadRequest);
            }
            catch (Exception ex)
            {
                return new UploadFileForManageResponse {  Success = false, StatusCode = 500,  Message = $"File upload failed: {ex.Message}" }; 
            }

            string fileUrl = uploadResult.url;

            var fileDto = await _manageRepository.UploadFileAsync(request.FileName, fileUrl, request.FileType, request.BrandId, request.CategoryId, request.ProductId, request.UserId);

            return new UploadFileForManageResponse
            {
                Id = fileDto.Id,
                FileName = fileDto.FileName,
                FileUrl = fileDto.FileUrl,
                FileType = fileDto.FileType,
                BrandId = fileDto.BrandId,
                CategoryId = fileDto.CategoryId,
                ProductId = fileDto.ProductId,
                Success = true,
                StatusCode = 200,
                Message = "File uploaded successfully"
            };
        }
    }
}
