using Domain.DTO.User;
using ErrorOr;
using FastEndpoints;

namespace Villager.Api.Service.Interface
{
    public interface IUserService
    {
        Task<ErrorOr<UserResponseDto>> CreateUser(CreateUserDto createUserDto);
        Task<ErrorOr<UserResponseDto>> EditUser(string id,EditUserDto editUserDto);
        Task<ErrorOr<UserResponseDto>> GetUser(string id);
        Task<ErrorOr<EmptyResponse>> DeleteUser(string id);
        Task<ErrorOr<String>> LoginUser(LoginUserDto loginUserDto);
        Task<ErrorOr<string>> GeneratePasswordResetToken(string email);
        Task<ErrorOr<bool>> ResetPassword(string code, string email, string password);
        Task<ErrorOr<string>> GenerateEmailConfirmationToken(string email);
        Task<ErrorOr<EmptyResponse>> VerifyEmail(string email, string code);




    }
}
