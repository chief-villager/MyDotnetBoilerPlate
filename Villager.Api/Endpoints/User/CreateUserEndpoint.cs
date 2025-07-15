using Domain.DTO.User;
using FastEndpoints;
using Villager.Api.Service.Interface;

namespace Villager.Api.Endpoints.User
{
    public class CreateUserEndpoint(
        IUserService userService
        ) : Endpoint<CreateUserDto>()
    {
        public override void Configure()
        {
            Post("/user/create");
            AllowAnonymous();

        }

        public override async Task HandleAsync(CreateUserDto req, CancellationToken ct)
        {
            var result = await userService.CreateUser(req);
            if (result.IsError)
            {
                await SendAsync(result.FirstError, statusCode: 400, ct);
                return;
            }
            await SendAsync(result.Value);

        }

    }
}
