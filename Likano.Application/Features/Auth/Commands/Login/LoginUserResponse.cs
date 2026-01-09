using Likano.Application.Common.Models;

namespace Likano.Application.Features.Auth.Commands.Login
{
    public class LoginUserResponse : BaseResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
