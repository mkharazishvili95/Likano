using MediatR;

namespace Likano.Application.Features.Auth.Commands.Login
{
    public class LoginUserCommand : IRequest<LoginUserResponse>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
