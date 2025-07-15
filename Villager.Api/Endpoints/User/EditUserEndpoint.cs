using Domain.DTO.User;
using FastEndpoints;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using Villager.Api.Service.Implementation;
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
            var response = await userService.EditUser(Id!,req);
            if (response.IsError)
            {
                await SendAsync(response.FirstError, statusCode:400, ct);
                return;
            }
            await SendAsync(response.Value);

        }
    }
}
