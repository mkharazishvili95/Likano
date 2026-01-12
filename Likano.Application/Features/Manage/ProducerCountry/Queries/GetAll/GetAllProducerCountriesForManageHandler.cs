using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Manage.ProducerCountry.Queries.GetAll
{
    public class GetAllProducerCountriesForManageHandler : IRequestHandler<GetAllProducerCountriesForManageQuery, GetAllProducerCountriesForManageResponse>
    {
        readonly IManageRepository _repository;
        public GetAllProducerCountriesForManageHandler(IManageRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetAllProducerCountriesForManageResponse> Handle(GetAllProducerCountriesForManageQuery request, CancellationToken cancellationToken)
        {
            var all = await _repository.GetAllProducerCountries() ?? new List<Domain.Entities.ProducerCountry>();

            if (request.Id.HasValue)
                all = all.Where(c => c.Id == request.Id.Value).ToList();
            if (!string.IsNullOrWhiteSpace(request.Name))
                all = all.Where(c => c.Name.Contains(request.Name.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();

            var total = all.Count;

            if (request.Pagination is { } p && p.PageNumber > 0 && p.PageSize > 0)
                all = all.Skip((p.PageNumber - 1) * p.PageSize).Take(p.PageSize).ToList();

            return new GetAllProducerCountriesForManageResponse
            {
                Success = true,
                StatusCode = 200,
                TotalCount = total,
                Items = all.Select(c => new GetAllProducerCountriesForManageItemsResponse
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList()
            };
        }
    }
}