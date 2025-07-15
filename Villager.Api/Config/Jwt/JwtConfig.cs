 using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Villager.Api.Config.Jwt
{
    public static class JwtConfig
    {
        public static void ConfigureAuthentication(
           this WebApplicationBuilder builder)
        {
           var  key =  builder.Configuration.GetValue<string>("Jwt:Key");


            builder.Services.AddAuthentication(scheme =>
            {

                scheme.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {

                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                };

            });
        }
    }
}