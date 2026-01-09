using MediatR;

namespace Likano.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommand : IRequest<LogoutResponse>
    {
        public bool RevokeAll { get; set; }
    }
}
