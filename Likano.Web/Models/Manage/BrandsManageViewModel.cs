using Likano.Application.Features.Manage.Brand.Queries.GetAll;

namespace Likano.Web.Models.Manage
{
    public class BrandsManageViewModel
    {
        public BrandsFilterVm Filter { get; set; } = new();
        public GetAllBrandsForManageResponse? Response { get; set; }
        public bool? IntoGrid { get; set; } = true;
    }
}
