using Likano.Application.Common.Models;

namespace Likano.Application.Features.Auth.Commands.Register
{
    public class RegisterUserResponse : BaseResponse
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}
