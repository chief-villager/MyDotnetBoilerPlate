using Domain.DTO.User;
using Domain.Entity;
using ErrorOr;
using FastEndpoints;
using Infrastructure.Services.Interface;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Villager.Api.Service.Interface;

namespace Villager.Api.Service.Implementation
{
    public class UserService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        SignInManager<ApplicationUser> signInManager,
        IEmailSenderService emailSender) : IUserService
    {
        public async Task<ErrorOr<UserResponseDto>> CreateUser(CreateUserDto createUserDto)
        {
            var findExistingUser = await userManager.FindByEmailAsync(createUserDto.Email);
            if (findExistingUser != null)
            {
               return  Error.Unauthorized(description: "Email already exist");
            };
            var date = DateOnly.Parse(createUserDto.DOB);
            var applicationUser = createUserDto.Adapt<ApplicationUser>();
            applicationUser.DOB = date;
            var result = await userManager.CreateAsync(applicationUser, createUserDto.Password);
            if (!result.Succeeded)
            {
                return Error.Failure(description: "Unable to create User.");

            }
            return applicationUser.Adapt<UserResponseDto>();


        }

        public async Task<ErrorOr<EmptyResponse>> DeleteUser( string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Error.NotFound(description: "User does not exist");
            }
            await userManager.DeleteAsync(user);
            return new ErrorOr<EmptyResponse>();
        }

        public async Task<ErrorOr<UserResponseDto>> EditUser(string id, EditUserDto editUserDto)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Error.NotFound(description: "User does not exist");
            }
            var updatedUser = editUserDto.Adapt(user);
            var result = await userManager.UpdateAsync(updatedUser);
            return result.Adapt<UserResponseDto>();

        }

        public async Task<ErrorOr<UserResponseDto>> GetUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Error.NotFound(description: "User does not exist");
            }
            return user.Adapt<UserResponseDto>();
        }

        public async Task<ErrorOr<String>> LoginUser(LoginUserDto loginUserDto)
        {
            var user = await userManager.FindByEmailAsync(loginUserDto.Email);
            if (user == null)
            {
                return Error.NotFound(description: "User Not Found");
            }
            var token = GenerateToken(user);
            if (token == null)
            {
                return Error.Failure(description: "Unable to generate token");
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, loginUserDto.Password, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                return Error.Unauthorized(description: "Login failed ,check email or password");
            }
            else if (result.IsLockedOut)
            {
                return Error.Unauthorized(description: "Account locked ");
            }

            return token;

        }

        public async Task<ErrorOr<string>> GeneratePasswordResetToken(string email)
        {
            var user = await FindUserByEmail(email);
            if (user.IsError)
            {
                return Error.NotFound(description: "User not found");
            }
            var passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(user.Value);
            emailSender.SendMessage(user.Value.Email!, "Password reset code", user.Value.UserName!, passwordResetToken);
            return passwordResetToken;
        }

        public async Task<ErrorOr<bool>> ResetPassword(string code, string email, string password)
        {
            var user = await FindUserByEmail(email);
            if (user.IsError)
            {
                return Error.NotFound(description: "User not found");
            }
            var result = await userManager.ResetPasswordAsync(user.Value, code, password);
            if (!result.Succeeded)
            {
                return Error.Failure(description: "Unable to change password");
            }
            return true;
        }

        public async Task<ErrorOr<string>> GenerateEmailConfirmationToken(string email)
        {
            var user = await FindUserByEmail(email);
            if (user.IsError)
            {
                return Error.NotFound(description: "User not found");
            }
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user.Value);
            emailSender.SendMessage(user.Value.Email!, "Email Confirmation code", user.Value.UserName!, token);
            return token;
            

        }
        public async Task<ErrorOr<EmptyResponse>> VerifyEmail(string email,string code)
        {
            var user = await FindUserByEmail(email);
            if (user.IsError)
            {
                return Error.NotFound(description: "User not found");
            }
            var result =  await userManager.ConfirmEmailAsync(user.Value, code);
            if (!result.Succeeded)
            {
                return Error.Failure(description: "Unable to confirm email");
            }

            return new ErrorOr<EmptyResponse>();
        }



        private string GenerateToken(ApplicationUser applicationUser)
        {
            var key = configuration.GetSection("Jwt:Key").Value;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {

                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email, applicationUser.Email),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Birthdate, applicationUser.DOB.ToString()),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.NameId, applicationUser.Id),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName, applicationUser.UserName) };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        private async Task<ErrorOr<ApplicationUser>> FindUserByEmail(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Error.NotFound(description: "User not found");
            }
            return user;
        }

    }
}