using Microsoft.Extensions.Configuration;

namespace Likano.Application.Common.Extensions
{
    public static class IConfigurationExtensions
    {
        public static string GetReadOnlyConnectionString(this IConfiguration configuration)
        {
            return configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentException();
        }
    }
}
