using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Manage.Product.Commands.ChangeCategory
{
    public class ChangeCategoryHandler : IRequestHandler<ChangeCategoryCommand, ChangeCategoryResponse>
    {
        readonly IManageRepository _repository;
        public ChangeCategoryHandler(IManageRepository repository)
        {
            _repository = repository;
        }

        public async Task<ChangeCategoryResponse> Handle(ChangeCategoryCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetProduct(request.ProductId);
            if(product == null)
                return new ChangeCategoryResponse { Message = "Product not found.", Success = false, StatusCode = 404 };

            var category = await _repository.GetCategory(request.NewCategoryId);

            if(category == null || category.IsActive.HasValue && category.IsActive.Value == false)
                return new ChangeCategoryResponse { Message = "Category not found or its not active.", Success = false, StatusCode = 404 };

            if(product.CategoryId == request.NewCategoryId)
                return new ChangeCategoryResponse { Message = "Product already in this category.", Success = false, StatusCode = 400 };

            product.CategoryId = request.NewCategoryId;
            await _repository.ChangeCategory(request.ProductId, request.NewCategoryId);

            return new ChangeCategoryResponse { Message = "კატეგორია წარმატებით შეიცვალა", Success = true, StatusCode = 200 };
        }
    }
}
