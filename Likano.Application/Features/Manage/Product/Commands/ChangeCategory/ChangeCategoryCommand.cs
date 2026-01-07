using MediatR;

namespace Likano.Application.Features.Manage.Product.Commands.ChangeCategory
{
    public class ChangeCategoryCommand : IRequest<ChangeCategoryResponse>
    {
        public int ProductId { get; set; }
        public int NewCategoryId { get; set; }
    }
}
