using Likano.Domain.Entities;

namespace Likano.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> Register(User user);
        Task<bool> UserNameExists(string userName);
    }
}
