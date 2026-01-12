using Likano.Application.Features.Manage.ProducerCountry.Queries.GetAll;

namespace Likano.Web.Models.Manage
{
    public class ProducerCountriesManageViewModel
    {
        public ProducerCountriesFilterVm Filter { get; set; } = new();
        public GetAllProducerCountriesForManageResponse Response { get; set; } = new();
    }
}