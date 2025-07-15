using Microsoft.AspNetCore.Identity;

namespace Villager.Api.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public required string DateOfBirth { get;  set; }
    }
}
