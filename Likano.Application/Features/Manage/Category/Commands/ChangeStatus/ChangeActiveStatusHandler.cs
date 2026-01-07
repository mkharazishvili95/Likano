using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Manage.Category.Commands.ChangeStatus
{
    public class ChangeActiveStatusHandler : IRequestHandler<ChangeActiveStatusCommand, ChangeActiveStatusResponse>
    {
        readonly IManageRepository _manageRepository;
        public ChangeActiveStatusHandler(IManageRepository manageRepository)
        {
            _manageRepository = manageRepository;
        }
        public async Task<ChangeActiveStatusResponse> Handle(ChangeActiveStatusCommand request, CancellationToken cancellationToken)
        {
            var category = await _manageRepository.GetCategory(request.CategoryId);
            if(category == null)
                return new ChangeActiveStatusResponse { Message = "Category not found", Success = false, StatusCode = 404 };

            var result =  await _manageRepository.ChangeActiveStatusCategory(request.CategoryId);
            return new ChangeActiveStatusResponse 
            { 
                StatusCode = result ? 200 : 500, Success = result, Message = result 
                ? "კატეგორიის სტატუსი წარმატებით შეიცვალა" 
                : "მოხდა შეცდომა" 
            };
        }
    }
}
