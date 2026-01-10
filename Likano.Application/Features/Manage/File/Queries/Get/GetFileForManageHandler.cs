using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Manage.File.Queries.Get
{
    public class GetFileForManageHandler : IRequestHandler<GetFileForManageQuery, GetFileForManageResponse>
    {
        readonly IManageRepository _manageRepository;
        public GetFileForManageHandler(IManageRepository manageRepository)
        {
            _manageRepository = manageRepository;
        }

        public async Task<GetFileForManageResponse> Handle(GetFileForManageQuery request, CancellationToken cancellationToken)
        {
            var file = await _manageRepository.GetFileAsync(request.FileId);

            if (file == null)
                return new GetFileForManageResponse { Message = "File not found.", Success = false, StatusCode = 404 };

            var response = new GetFileForManageResponse
            {
                StatusCode = 200,
                Success = true,
                Id = file.Id,
                FileName = file.FileName,
                FileUrl = file.FileUrl,
                FileType = file.FileType,
                UploadDate = file.UploadDate,
                UserId = file.UserId,
                MainImage = file.MainImage,
                CategoryId = file.CategoryId,
                ProductId = file.ProductId,
                BrandId = file.BrandId
            };

            return response;
        }
    }
}
