using Likano.Application.Interfaces;
using Likano.Domain.Entities;
using Likano.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;


namespace Likano.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        readonly ApplicationDbContext _db;
        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> UserNameExists(string userName) => await _db.Users.AnyAsync(u => u.UserName.ToUpper() == userName.ToUpper());
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
    }
}
