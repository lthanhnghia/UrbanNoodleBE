
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UrbanNoodle.ApplicationContext;
using UrbanNoodle.Middleware;
using UrbanNoodle.Service.Interface;
using UrbanNoodle.Service;
using DotNetEnv;
namespace UrbanNoodle
{
    public class Program
    {
        public static void Main(string[] args)
        {

            Env.Load();
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
   options.UseNpgsql(Environment.GetEnvironmentVariable("DEFAULT_CONNECTION")));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                        ValidateAudience = true,
                        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                             Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!)
                        )
                    };
                });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IAccountService,AccountService >();
            builder.Services.AddScoped<IAuthService, AuthService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
