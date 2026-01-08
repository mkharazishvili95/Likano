using Likano.Application.Features.Manage.Brand.Commands.Change;
using Likano.Application.Features.Manage.Brand.Queries.GetAll;
using Likano.Application.Features.Manage.Category.Commands.ChangeStatus;
using Likano.Application.Features.Manage.Category.Queries.Get;
using Likano.Application.Features.Manage.Category.Queries.GetAll;
using Likano.Application.Features.Manage.Product.Commands.ChangeCategory;
using Likano.Application.Features.Manage.Product.Commands.ChangeStatus;
using Likano.Application.Features.Manage.Product.Queries.Get;
using Likano.Application.Features.Manage.Product.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Likano.Controllers
{
    [ApiController]
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
    }
}
