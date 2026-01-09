using Likano.Application.Interfaces;
using Likano.Domain.Entities;
using Likano.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace Likano.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        readonly ApplicationDbContext _db;
        readonly IHttpContextAccessor _httpContextAccessor;
        public UserRepository(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> UserNameExists(string userName) => await _db.Users.AnyAsync(u => u.UserName.ToUpper() == userName.ToUpper());
        public async Task<User?> GetByIdAsync(int id) => await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        public async Task<User?> GetByUserNameAsync(string userName) => await _db.Users.FirstOrDefaultAsync(u => u.UserName.ToUpper() == userName.ToUpper());
        public async Task<User?> Register(User user)
        {
            try
            {
                var newUser = new User
                {
                    UserName = user.UserName,
                    Password = user.Password,
                    UserType = Domain.Enums.User.UserType.User,
                    RefreshToken = null,
                    RefreshTokenExpiry = null
                };

                _db.Users.Add(newUser);
                await _db.SaveChangesAsync();

                return newUser;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
        public async Task UpdateRefreshTokenAsync(User user, string refreshToken, DateTime expiry)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = expiry;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }

        public ClaimsPrincipal? GetUser() => _httpContextAccessor.HttpContext?.User;

        public int? GetUserId()
        {
            var user = GetUser();
            if (user?.Identity is not { IsAuthenticated: true }) return null;

            var claim = user.FindFirst(ClaimTypes.NameIdentifier)
                        ?? user.FindFirst("sub")
                        ?? user.FindFirst("userId");

            if (claim == null || !int.TryParse(claim.Value, out var userId)) return null;

            return userId;
        }

        public async Task RevokeRefreshTokenAsync(User user)
        {
            var tracked = await _db.Users.FindAsync(user.Id);
            if (tracked == null) return;
            tracked.RefreshToken = null;
            tracked.RefreshTokenExpiry = null;
            await _db.SaveChangesAsync();
        }
    }
}
