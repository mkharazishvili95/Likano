using Likano.Infrastructure.Queries.Product.Models;
using Likano.Infrastructure.Queries.Product.Models.Details;
using Microsoft.Extensions.Configuration;

namespace Likano.Infrastructure.Queries.Product
{
    public class ProductQueries : QueriesBase, IProductQueries
    {
        public ProductQueries(IConfiguration configuration) : base(configuration) { }

        public async Task<GetAllProductsForSearchResponse> GetAllProductsForSearch(GetAllProductsForSearchQuery request)
        {
            var whereClause = request.ToWhereClause();

            var orderBy = request.SortBy switch
            {
                "PriceAsc" => "p.Price ASC",
                "PriceDesc" => "p.Price DESC",
                "CreateDateDesc" => "p.CreateDate DESC",
                _ => "p.CreateDate DESC"
            };

            var offset = (request.Pagination.PageNumber - 1) * request.Pagination.PageSize;

            var countQuery = $@"
                SELECT COUNT(*) AS TotalCount
                FROM Products p
                LEFT JOIN Categories c ON c.Id = p.CategoryId
                LEFT JOIN Brands b ON b.Id = p.BrandId
                LEFT JOIN ProducerCountries pc ON pc.Id = p.ProducerCountryId
                WHERE {whereClause}";

            var totalCount = await Get(countQuery, reader => reader.GetInt32(reader.GetOrdinal("TotalCount")));

            var commandText = $@"
                SELECT 
                    p.Id,
                    p.Title,
                    p.Description,
                    p.Price,
                    p.IsAvailable,
                    p.ImageUrl,
                    p.CreateDate,
                    p.Length,
                    p.Width,
                    p.Height,
                    p.Color,
                    p.CategoryId,
                    c.Name AS CategoryTitle,
                    c.Logo AS CategoryLogo,
                    p.ProducerCountryId,
                    pc.Name AS ProducerCountryName,
                    p.BrandId,
                    b.Name AS BrandTitle,
                    b.Logo AS BrandLogo
                FROM Products p
                LEFT JOIN Categories c ON c.Id = p.CategoryId
                LEFT JOIN ProducerCountries pc ON pc.Id = p.ProducerCountryId
                LEFT JOIN Brands b ON b.Id = p.BrandId
                WHERE {whereClause}
                ORDER BY {orderBy}
                OFFSET {offset} ROWS FETCH NEXT {request.Pagination.PageSize} ROWS ONLY";

            var items = await GetMany(commandText, reader => new GetAllProductsForSearchItemsResponse
            {
                Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? null : reader.GetInt32(reader.GetOrdinal("Id")),
                Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString(reader.GetOrdinal("Title")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                Price = reader.IsDBNull(reader.GetOrdinal("Price")) ? null : reader.GetDecimal(reader.GetOrdinal("Price")),
                IsAvailable = reader.IsDBNull(reader.GetOrdinal("IsAvailable")) ? null : reader.GetBoolean(reader.GetOrdinal("IsAvailable")),
                ProductImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl")),
                CreateDate = reader.IsDBNull(reader.GetOrdinal("CreateDate")) ? null : reader.GetDateTime(reader.GetOrdinal("CreateDate")),
                Length = reader.IsDBNull(reader.GetOrdinal("Length")) ? null : reader.GetDecimal(reader.GetOrdinal("Length")),
                Width = reader.IsDBNull(reader.GetOrdinal("Width")) ? null : reader.GetDecimal(reader.GetOrdinal("Width")),
                Height = reader.IsDBNull(reader.GetOrdinal("Height")) ? null : reader.GetDecimal(reader.GetOrdinal("Height")),
                Color = reader.IsDBNull(reader.GetOrdinal("Color")) ? null : reader.GetString(reader.GetOrdinal("Color")),
                CategoryId = reader.IsDBNull(reader.GetOrdinal("CategoryId")) ? null : reader.GetInt32(reader.GetOrdinal("CategoryId")),
                Category = reader.IsDBNull(reader.GetOrdinal("CategoryId")) ? null : new CategoryDtoForSearch
                {
                    CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                    Name = reader.IsDBNull(reader.GetOrdinal("CategoryTitle")) ? null : reader.GetString(reader.GetOrdinal("CategoryTitle")),
                    Logo = reader.IsDBNull(reader.GetOrdinal("CategoryLogo")) ? null : reader.GetString(reader.GetOrdinal("CategoryLogo"))
                },
                ProducerCountryId = reader.IsDBNull(reader.GetOrdinal("ProducerCountryId")) ? null : reader.GetInt32(reader.GetOrdinal("ProducerCountryId")),
                ProducerCountry = reader.IsDBNull(reader.GetOrdinal("ProducerCountryId")) ? null : new ProducerCountryDtoForSearch
                {
                    ProducerCountryId = reader.GetInt32(reader.GetOrdinal("ProducerCountryId")),
                    Name = reader.IsDBNull(reader.GetOrdinal("ProducerCountryName")) ? null : reader.GetString(reader.GetOrdinal("ProducerCountryName"))
                },
                BrandId = reader.IsDBNull(reader.GetOrdinal("BrandId")) ? null : reader.GetInt32(reader.GetOrdinal("BrandId")),
                Brand = reader.IsDBNull(reader.GetOrdinal("BrandId")) ? null : new BrandDtoForSearch
                {
                    BrandId = reader.GetInt32(reader.GetOrdinal("BrandId")),
                    Name = reader.IsDBNull(reader.GetOrdinal("BrandTitle")) ? null : reader.GetString(reader.GetOrdinal("BrandTitle")),
                    Logo = reader.IsDBNull(reader.GetOrdinal("BrandLogo")) ? null : reader.GetString(reader.GetOrdinal("BrandLogo"))
                }
            });

            var productIds = items.Select(x => x.Id).Where(x => x.HasValue).Select(x => x.Value).ToList();
            if (productIds.Any())
            {
                var ids = string.Join(",", productIds);
                var imagesQuery = $@"
                SELECT 
                    f.Id AS ImageId,
                    f.FileName,
                    f.FileUrl,
                    f.MainImage,
                    f.ProductId
                FROM Files f
                WHERE f.ProductId IN ({ids}) AND f.IsDeleted = 0 AND f.FileType = 1";

                var images = await GetMany(imagesQuery, reader => new ImageDtoForSearch
                {
                    ImageId = reader.GetInt32(reader.GetOrdinal("ImageId")),
                    FileName = reader.IsDBNull(reader.GetOrdinal("FileName")) ? null : reader.GetString(reader.GetOrdinal("FileName")),
                    FileUrl = reader.IsDBNull(reader.GetOrdinal("FileUrl")) ? null : reader.GetString(reader.GetOrdinal("FileUrl")),
                    MainImage = reader.IsDBNull(reader.GetOrdinal("MainImage")) ? null : reader.GetBoolean(reader.GetOrdinal("MainImage")),
                    ProductId = reader.GetInt32(reader.GetOrdinal("ProductId"))
                });

                var imagesByProduct = images
                    .GroupBy(img => img.ProductId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                foreach (var item in items)
                {
                    item.Images = item.Id.HasValue && imagesByProduct.TryGetValue(item.Id.Value, out var imgs)
                        ? imgs
                        : new List<ImageDtoForSearch>();
                }
            }
            else
            {
                foreach (var item in items)
                    item.Images = new List<ImageDtoForSearch>();
            }

            return new GetAllProductsForSearchResponse
            {
                TotalCount = totalCount,
                Items = items
            };
        }

        public async Task<GetProductDetailsResponse> GetProductDetails(GetProductDetailsQuery request)
        {
            var commandText = $@"
            SELECT 
            p.Id,
            p.Title,
            p.Description,
            p.Price,
            p.IsAvailable,
            p.ImageUrl,
            p.CreateDate,
            p.Length,
            p.Width,
            p.Height,
            p.Color,
            p.CategoryId,
            c.Name AS CategoryTitle,
            c.Logo AS CategoryLogo,
            p.ProducerCountryId,
            pc.Name AS ProducerCountryName,
            p.BrandId,
            b.Name AS BrandTitle,
            b.Logo AS BrandLogo
            FROM Products p
            LEFT JOIN Categories c ON c.Id = p.CategoryId
            LEFT JOIN ProducerCountries pc ON pc.Id = p.ProducerCountryId
            LEFT JOIN Brands b ON b.Id = p.BrandId
            WHERE p.Id = {request.ProductId}";

            var items = await GetMany(commandText, reader => new GetProductDetailsResponse
            {
                Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? null : reader.GetInt32(reader.GetOrdinal("Id")),
                Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString(reader.GetOrdinal("Title")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                Price = reader.IsDBNull(reader.GetOrdinal("Price")) ? null : reader.GetDecimal(reader.GetOrdinal("Price")),
                IsAvailable = reader.IsDBNull(reader.GetOrdinal("IsAvailable")) ? null : reader.GetBoolean(reader.GetOrdinal("IsAvailable")),
                ProductImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl")),
                CreateDate = reader.IsDBNull(reader.GetOrdinal("CreateDate")) ? null : reader.GetDateTime(reader.GetOrdinal("CreateDate")),
                Length = reader.IsDBNull(reader.GetOrdinal("Length")) ? null : reader.GetDecimal(reader.GetOrdinal("Length")),
                Width = reader.IsDBNull(reader.GetOrdinal("Width")) ? null : reader.GetDecimal(reader.GetOrdinal("Width")),
                Height = reader.IsDBNull(reader.GetOrdinal("Height")) ? null : reader.GetDecimal(reader.GetOrdinal("Height")),
                Color = reader.IsDBNull(reader.GetOrdinal("Color")) ? null : reader.GetString(reader.GetOrdinal("Color")),
                CategoryId = reader.IsDBNull(reader.GetOrdinal("CategoryId")) ? null : reader.GetInt32(reader.GetOrdinal("CategoryId")),
                Category = reader.IsDBNull(reader.GetOrdinal("CategoryId")) ? null : new CategoryDtoForSearch
                {
                    CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                    Name = reader.IsDBNull(reader.GetOrdinal("CategoryTitle")) ? null : reader.GetString(reader.GetOrdinal("CategoryTitle")),
                    Logo = reader.IsDBNull(reader.GetOrdinal("CategoryLogo")) ? null : reader.GetString(reader.GetOrdinal("CategoryLogo"))
                },
                ProducerCountryId = reader.IsDBNull(reader.GetOrdinal("ProducerCountryId")) ? null : reader.GetInt32(reader.GetOrdinal("ProducerCountryId")),
                ProducerCountry = reader.IsDBNull(reader.GetOrdinal("ProducerCountryId")) ? null : new ProducerCountryDtoForSearch
                {
                    ProducerCountryId = reader.GetInt32(reader.GetOrdinal("ProducerCountryId")),
                    Name = reader.IsDBNull(reader.GetOrdinal("ProducerCountryName")) ? null : reader.GetString(reader.GetOrdinal("ProducerCountryName"))
                },
                BrandId = reader.IsDBNull(reader.GetOrdinal("BrandId")) ? null : reader.GetInt32(reader.GetOrdinal("BrandId")),
                Brand = reader.IsDBNull(reader.GetOrdinal("BrandId")) ? null : new BrandDtoForSearch
                {
                    BrandId = reader.GetInt32(reader.GetOrdinal("BrandId")),
                    Name = reader.IsDBNull(reader.GetOrdinal("BrandTitle")) ? null : reader.GetString(reader.GetOrdinal("BrandTitle")),
                    Logo = reader.IsDBNull(reader.GetOrdinal("BrandLogo")) ? null : reader.GetString(reader.GetOrdinal("BrandLogo"))
                }
            });

            var product = items.FirstOrDefault();
            if (product == null)
            {
                return new GetProductDetailsResponse
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Product not found"
                };
            }

            if (product.Id.HasValue)
            {
                var imagesQuery = $@"
                SELECT 
                f.Id AS ImageId,
                f.FileName,
                f.FileUrl,
                f.MainImage,
                f.ProductId
                FROM Files f
                WHERE f.ProductId = {product.Id.Value} AND f.IsDeleted = 0 AND f.FileType = 1";

                var images = await GetMany(imagesQuery, reader => new ImageDtoForSearch
                {
                    ImageId = reader.GetInt32(reader.GetOrdinal("ImageId")),
                    FileName = reader.IsDBNull(reader.GetOrdinal("FileName")) ? null : reader.GetString(reader.GetOrdinal("FileName")),
                    FileUrl = reader.IsDBNull(reader.GetOrdinal("FileUrl")) ? null : reader.GetString(reader.GetOrdinal("FileUrl")),
                    MainImage = reader.IsDBNull(reader.GetOrdinal("MainImage")) ? null : reader.GetBoolean(reader.GetOrdinal("MainImage")),
                    ProductId = reader.GetInt32(reader.GetOrdinal("ProductId"))
                });

                product.Images = images;
            }
            else
            {
                product.Images = new List<ImageDtoForSearch>();
            }

            product.Success = true;
            product.StatusCode = 200;
            product.Message = "OK";
            return product;
        }
    }
}