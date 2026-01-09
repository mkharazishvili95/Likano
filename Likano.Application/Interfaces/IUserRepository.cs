using Likano.Domain.Entities;

namespace Likano.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> Register(User user);
        Task<bool> UserNameExists(string userName);
        Task<User?> GetByUserNameAsync(string userName);
        Task UpdateRefreshTokenAsync(User user, string refreshToken, DateTime expiry);
    }
}
