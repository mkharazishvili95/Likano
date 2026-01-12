using Likano.Application.Features.Manage.Brand.Commands.Change;
using Likano.Application.Features.Manage.Brand.Commands.ChangeStatus;
using Likano.Application.Features.Manage.Brand.Commands.Create;
using Likano.Application.Features.Manage.Brand.Commands.Edit;
using Likano.Application.Features.Manage.Brand.Queries.Get;
using Likano.Application.Features.Manage.Brand.Queries.GetAll;
using Likano.Application.Features.Manage.Category.Commands.ChangeStatus;
using Likano.Application.Features.Manage.Category.Commands.Create;
using Likano.Application.Features.Manage.Category.Commands.Edit;
using Likano.Application.Features.Manage.Category.Queries.Get;
using Likano.Application.Features.Manage.Category.Queries.GetAll;
using Likano.Application.Features.Manage.Product.Commands.ChangeCategory;
using Likano.Application.Features.Manage.Product.Commands.ChangeStatus;
using Likano.Application.Features.Manage.Product.Queries.Get;
using Likano.Application.Features.Manage.Product.Queries.GetAll;
using Likano.Domain.Enums.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Likano.Controllers
{
    [ApiController]
    [Authorize(Roles = nameof(UserType.Admin))]
    [Route("api/[controller]")]
    public class ManageController : ControllerBase
    {
        readonly IMediator _mediator;
        public ManageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("product/{id}")]
        public async Task<GetProductForManageResponse> GetProduct(int id)
            => await _mediator.Send(new GetProductForManageQuery { Id = id });

        [HttpPost("products")]
        public async Task<GetAllProductsForManageResponse> GetAllProducts([FromBody] GetAllProductsForManageQuery query)
            => await _mediator.Send(query);

        [HttpPost("product/status")]
        public async Task<ChangeProductStatusResponse> ChangeProductStatus([FromBody] ChangeProductStatusCommand command)
            => await _mediator.Send(command);

        [HttpPost("categories")]
        public async Task<GetAllCategoriesForManageResponse> GetAllCategories([FromBody] GetAllCategoriesForManageQuery query)
            => await _mediator.Send(query);

        [HttpGet("category/{id}")]
        public async Task<GetCategoryForManageResponse> GetCategory(int id)
            => await _mediator.Send(new GetCategoryForManageQuery { CategoryId = id });

        [HttpPost("category/status")]
        public async Task<ChangeActiveStatusResponse> ChangeCategoryStatus([FromBody] ChangeActiveStatusCommand command)
            => await _mediator.Send(command);

        [HttpPost("product/change-category")]
        public async Task<ChangeCategoryResponse> ChangeProductCategory([FromBody] ChangeCategoryCommand command)
            => await _mediator.Send(command);

        [HttpPost("product/change-brand")]
        public async Task<ChangeBrandResponse> ChangeProductBrand([FromBody] ChangeBrandCommand command)
            => await _mediator.Send(command);

        [HttpPost("brands")]
        public async Task<GetAllBrandsForManageResponse> GetAllBrands([FromBody] GetAllBrandsForManageQuery query)
            => await _mediator.Send(query);

        [HttpGet("brand/{id}")]
        public async Task<GetBrandForManageResponse> GetBrand(int id)
            => await _mediator.Send(new GetBrandForManageQuery { BrandId = id });

        [HttpPost("brand/status")]
        public async Task<ChangeBrandActiveStatusResponse> ChangeBrandStatus([FromBody] ChangeBrandActiveStatusCommand command)
            => await _mediator.Send(command);

        [HttpPost("create/category")]
        public async Task<CreateCategoryForManageResponse> CreateCategory([FromBody] CreateCategoryForManageCommand request)
            => await _mediator.Send(request);

        [HttpPost("edit/category")]
        public async Task<EditCategoryForManageResponse> EditCategory([FromBody] EditCategoryForManageCommand request)
        => await _mediator.Send(request);

        [HttpPost("create/brand")]
        public async Task<CreateBrandForManageResponse> CreateBrand([FromBody] CreateBrandForManageCommand request)
            => await _mediator.Send(request);

        [HttpPost("edit/brand")]
        public async Task<EditBrandForManageResponse> EditBrand([FromBody] EditBrandForManageCommand request)
            => await _mediator.Send(request);
    }
}
