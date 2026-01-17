using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.ProducerCountry.Queries.GetAll
{
    public class GetAllProducerCountriesHandler : IRequestHandler<GetAllProducerCountriesQuery, GetAllProducerCountriesResponse>
    {
        readonly IProducerCountryRepository _repository;
        public GetAllProducerCountriesHandler(IProducerCountryRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetAllProducerCountriesResponse> Handle(GetAllProducerCountriesQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.Pagination?.PageNumber > 0 ? request.Pagination.PageNumber : 1;
            var pageSize = request.Pagination?.PageSize > 0 ? request.Pagination.PageSize : 20;
            var search = request.SearchString?.Trim();

            var countries = await _repository.GetAll() ?? new List<Domain.Entities.ProducerCountry>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                countries = countries
                    .Where(c => (!string.IsNullOrEmpty(c.Name) && c.Name.Contains(search, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            if (request.Id != null)
            {
                countries = countries
                    .Where(c => c.Id == request.Id)
                    .ToList();
            }

            var totalCount = countries.Count;

            var paged = countries
                .OrderBy(c => c.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new GetAllProducerCountriesResponse
            {
                Success = true,
                StatusCode = 200,
                TotalCount = totalCount,
                Items = paged.Select(c => new GetAllProducerCountriesItemsResponse
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList()
            };
        }
    }
}
