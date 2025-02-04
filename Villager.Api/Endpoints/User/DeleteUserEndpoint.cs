using Domain.DTO.User;
using ErrorOr;
using FastEndpoints;
using Villager.Api.Service.Interface;

namespace Villager.Api.Endpoints.User
{
    public class DeleteUserEndpoint(
        IUserService userService
      ) : EndpointWithoutRequest
    {
        public override void Configure()
        {
            Delete("/user/{id}");
            AllowAnonymous();
        }
        public override async Task HandleAsync( CancellationToken ct)
        {
            var Id = Route<string>("id");
            var response = await userService.DeleteUser(Id!);
            if (response.IsError)
            {
                await SendAsync(response.Errors, statusCode: 400);
                return;
            }



        }
    }
}
