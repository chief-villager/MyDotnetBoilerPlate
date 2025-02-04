using Domain.DTO.User;
using FastEndpoints;
using Grpc.Core;
using Villager.Api.Data;
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
            var response = await userService.GetUser(Id!);
            if (response.IsError)
            {
                await SendAsync(response.FirstError, statusCode: 400,ct);
                return;
            }
            await SendAsync(response.Value,statusCode:200);

        }
    }
}
