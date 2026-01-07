using Likano.Application.Features.Manage.Category.Commands.ChangeStatus;
using Likano.Application.Features.Manage.Category.Queries.GetAll;
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

        [HttpPost("category/status")]
        public async Task<ChangeActiveStatusResponse> ChangeCategoryStatus([FromBody] ChangeActiveStatusCommand command)
            => await _mediator.Send(command);
    }
}
