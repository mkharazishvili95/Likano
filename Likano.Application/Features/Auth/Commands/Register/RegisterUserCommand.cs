using MediatR;

namespace Likano.Application.Features.Auth.Commands.Register
{
    public class RegisterUserCommand : IRequest<RegisterUserResponse>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
