using FastEndpoints;
using Villager.Api.Domain.DTO;
using Villager.Api.Service.Interface;

namespace Villager.Api.Endpoints.User
{
    public class EditUserEndpoint(
         IUserService userService
        ) : Endpoint<EditUserDto>
    {
        public override void Configure()
        {
            Put("/user/edit/{id}");
        }

        public override async Task HandleAsync(EditUserDto req, CancellationToken ct)
        {
            var Id = Route<string>("id");
            if (string.IsNullOrWhiteSpace(Id))
            {
                await SendAsync(new { Error = "User ID is required" }, 400, ct);
                return;
            }
            var response = await userService.EditUser(Id,req);
            if (response.IsError)
            {
                await SendAsync(response.FirstError, statusCode:400, ct);
                return;
            }
            await SendAsync(response.Value, statusCode:200, cancellation: ct);

        }
    }
}
