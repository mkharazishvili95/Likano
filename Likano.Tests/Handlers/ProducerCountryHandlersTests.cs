using Likano.Application.Features.Manage.ProducerCountry.Commands.Create;
using Likano.Application.Features.Manage.ProducerCountry.Commands.Edit;
using Likano.Application.Features.Manage.ProducerCountry.Queries.GetAll;
using Likano.Domain.Entities;
using Likano.Infrastructure.Data;
using Likano.Infrastructure.Repositories;
using Likano.Tests.Helpers;
using Microsoft.Extensions.Options;

namespace Likano.Tests.Handlers
{
    [TestFixture]
    public class ProducerCountryHandlersTests
    {
        ApplicationDbContext _db = null!;
        ManageRepository _repo = null!;
        int _countryId;

        [SetUp]
        public async Task SetUp()
        {
            _db = DbContextHelper.GetInMemoryDbContext();
            var imageKitSettings = Options.Create(new Likano.Application.Configuration.ImageKitSettings());
            _repo = new ManageRepository(_db, imageKitSettings);

            var country = new ProducerCountry { Name = "Georgia" };
            _db.ProducerCountries.Add(country);
            await _db.SaveChangesAsync();
            _countryId = country.Id;
        }

        [TearDown]
        public void TearDown() => _db.Dispose();

        [Test]
        public async Task GetAllProducerCountriesForManageHandler_ReturnsCountries()
        {
            var handler = new GetAllProducerCountriesForManageHandler(_repo);
            var query = new GetAllProducerCountriesForManageQuery { Pagination = new Likano.Application.Common.Models.Pagination { PageNumber = 1, PageSize = 10 } };

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Items, Is.Not.Null);
            Assert.That(result.Items!.Count, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public async Task CreateProducerCountryForManageHandler_ValidName_ReturnsSuccess()
        {
            var handler = new CreateProducerCountryForManageHandler(_repo);
            var command = new CreateProducerCountryForManageCommand { Name = "Germany" };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(201));
            Assert.That(result.Id, Is.GreaterThan(0));
        }

        [Test]
        public async Task EditProducerCountryForManageHandler_ValidData_ReturnsSuccess()
        {
            var handler = new EditProducerCountryForManageHandler(_repo);
            var command = new EditProducerCountryForManageCommand { Id = _countryId, Name = "Updated Country" };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }
    }
}