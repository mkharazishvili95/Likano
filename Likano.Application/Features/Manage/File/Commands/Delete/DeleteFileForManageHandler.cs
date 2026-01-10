using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Manage.File.Commands.Delete
{
    public class DeleteFileForManageHandler : IRequestHandler<DeleteFileForManageCommand, DeleteFileForManageResponse>
    {
        readonly IManageRepository _manageRepository;
        public DeleteFileForManageHandler(IManageRepository manageRepository)
        {
            _manageRepository = manageRepository;
        }

        public async Task<DeleteFileForManageResponse> Handle(DeleteFileForManageCommand request, CancellationToken cancellationToken)
        {
            var file = await _manageRepository.GetFileAsync(request.FileId);

            if(file == null)
                return new DeleteFileForManageResponse { Success = false, StatusCode = 404, Message = "File not found" };   

            if(file.IsDeleted)
                return new DeleteFileForManageResponse { Success = false, StatusCode = 400, Message = "File already deleted" };

            await _manageRepository.DeleteFileAsync(request.FileId);
            return new DeleteFileForManageResponse { Success = true, StatusCode = 200, Message = "File deleted successfully" };
        }
    }
}
