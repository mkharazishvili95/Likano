using Likano.Application.Common.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Likano.Infrastructure.Queries
{
    public abstract class QueriesBase
    {
        string connectionString { get; set; }
        public QueriesBase(IConfiguration configuration)
        {
            connectionString = configuration.GetReadOnlyConnectionString();
        }
        public async Task<TResponse> Get<TResponse>(string commandText, Func<SqlDataReader, TResponse> selector) where TResponse : new()
        {
            var response = new TResponse();
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    using var reader = await command.ExecuteReaderAsync();
                    if (reader.HasRows)
                        while (await reader.ReadAsync())
                        {
                            response = await reader.Select(selector);
                        }
                }
                await connection.CloseAsync();
            }
            return response;
        }

        public async Task<List<TResponse>> GetMany<TResponse>(string commandText, Func<SqlDataReader, TResponse> selector)
        {
            var response = new List<TResponse>();
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    using var reader = await command.ExecuteReaderAsync();
                    if (reader.HasRows)
                        while (await reader.ReadAsync())
                        {
                            var value = await reader.Select(selector);
                            response.Add(value);
                        }
                }
                await connection.CloseAsync();
            }
            return response;
        }
    }

    public static class QuerieHelpler
    {
        public static async Task<T> Select<T>(this SqlDataReader reader, Func<SqlDataReader, T> selector)
        {
            return selector.Invoke(reader);
        }
    }
}