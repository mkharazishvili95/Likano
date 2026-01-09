using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Auth.Commands.Logout
{
    public class LogoutHandler : IRequestHandler<LogoutCommand, LogoutResponse>
    {
        readonly IUserRepository _userRepository;
        public LogoutHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<LogoutResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var userId = _userRepository.GetUserId();
            if (userId == null)
                return new LogoutResponse { Success = false, StatusCode = 401, Message = "Unauthorized" };

            var user = await _userRepository.GetByIdAsync(userId.Value);
            if (user == null)
                return new LogoutResponse { Success = false, StatusCode = 404, Message = "User not found" };

            await _userRepository.RevokeRefreshTokenAsync(user);
            return new LogoutResponse { Success = true, StatusCode = 200, Message = "Logged out" };
        }
    }
}
