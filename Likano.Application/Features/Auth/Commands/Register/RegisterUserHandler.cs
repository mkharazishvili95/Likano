using Likano.Application.Interfaces;
using Likano.Domain.Enums.User;
using MediatR;

namespace Likano.Application.Features.Auth.Commands.Register
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
    {
        readonly IUserRepository _userRepository;
        readonly RegisterUserValidator _validator;

        public RegisterUserHandler(IUserRepository userRepository, RegisterUserValidator validator)
        {
            _userRepository = userRepository;
            _validator = validator;
        }

        public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return new RegisterUserResponse { Success = false, Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)), StatusCode = 400 };

            bool alreadyExists = await _userRepository.UserNameExists(request.UserName);

            if (alreadyExists)
                return new RegisterUserResponse { Success = false, Message = $"{request.UserName} already exists.", StatusCode = 409 };


            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new Likano.Domain.Entities.User
            {
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password), 
                UserName = request.UserName
            };

            var createdUser = await _userRepository.Register(newUser);

            if(createdUser == null)
                return new RegisterUserResponse { Success = false, Message = "Failed to register user", StatusCode = 500 };

            return new RegisterUserResponse
            {
                Success = true,
                Message = "User registered successfully",
                StatusCode = 200,
                UserName = createdUser.UserName,
                UserId = createdUser.Id
            };
        }
    }
}
