using Likano.Domain.Entities;
using System.Security.Claims;

namespace Likano.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> Register(User user);
        Task<bool> UserNameExists(string userName);
        Task<User?> GetByUserNameAsync(string userName);
        Task UpdateRefreshTokenAsync(User user, string refreshToken, DateTime expiry);
        int? GetUserId();
        ClaimsPrincipal? GetUser();
        Task<User?> GetByIdAsync(int id);
        Task RevokeRefreshTokenAsync(User user);
    }
}
