namespace Likano.Web.Models.Manage
{
    public class ProducerCountriesFilterVm
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? Id { get; set; }
        public string? Name { get; set; }
    }
}