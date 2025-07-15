using FastEndpoints;
using Villager.Api.Service.Interface;

namespace Villager.Api.Endpoints.User
{
    public class GetUserEndpoint(
        IUserService userService
        ) : EndpointWithoutRequest
    {
        public override void Configure()
        {
            Get("/user/{id}");
            AllowAnonymous();
        }
        public override async Task HandleAsync( CancellationToken ct)
        {

            var Id = Route<string>("id");
            if (string.IsNullOrWhiteSpace(Id))
            {
                await SendAsync(new { Error = "User ID is required" }, 400, ct);
                return;
            }
            var response = await userService.GetUser(Id!);
            if (response.IsError)
            {
                await SendAsync(response.FirstError, statusCode: 400,ct);
                return;
            }
            await SendAsync(response.Value, cancellation: ct);

        }
    }
}
