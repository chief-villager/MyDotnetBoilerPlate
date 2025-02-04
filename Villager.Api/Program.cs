using Domain.DTO.User;
using Domain.Entity;
using FastEndpoints;
using FluentValidation;
using Infrastructure.Configuration.Seeding;
using Infrastructure.Data;
using Infrastructure.Services.Implementation;
using Infrastructure.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Villager.Api.Config.Jwt;
using Villager.Api.Service.Implementation;
using Villager.Api.Service.Interface;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
SmtpSettings? smtp = builder.Configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
builder.Services.AddOptions<SmtpSettings>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddIdentity<ApplicationUser,IdentityRole>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;

}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

JwtConfig.ConfigureAuthentication(builder);
builder.Services.AddAuthorization();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
builder.Services
   .AddFastEndpoints();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
    try
    {
       
        AdminSeeder.AdminDataSeederAsync(scope.ServiceProvider);

    }
    catch( Exception ex )
    {
       
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the data.");
    }


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseFastEndpoints();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
