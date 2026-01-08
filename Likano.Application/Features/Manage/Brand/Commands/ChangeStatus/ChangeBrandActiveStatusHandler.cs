using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Manage.Brand.Commands.ChangeStatus
{
    public class ChangeBrandActiveStatusHandler : IRequestHandler<ChangeBrandActiveStatusCommand, ChangeBrandActiveStatusResponse>
    {
        readonly IManageRepository _manageRepository;
        public ChangeBrandActiveStatusHandler(IManageRepository manageRepository)
        {
            _manageRepository = manageRepository;
        }
        public async Task<ChangeBrandActiveStatusResponse> Handle(ChangeBrandActiveStatusCommand request, CancellationToken cancellationToken)
        {
            var brand = await _manageRepository.GetBrand(request.BrandId);
            if (brand == null)
                return new ChangeBrandActiveStatusResponse { Message = "Brand not found", Success = false, StatusCode = 404 };

            var result = await _manageRepository.ChangeActiveStatusBrand(request.BrandId);
            return new ChangeBrandActiveStatusResponse
            {
                StatusCode = result ? 200 : 500,
                Success = result,
                Message = result
                ? "ბრენდის სტატუსი წარმატებით შეიცვალა"
                : "მოხდა შეცდომა"
            };
        }
    }
}
