
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UrbanNoodle.ApplicationContext;
using UrbanNoodle.Middleware;
using UrbanNoodle.Service.Interface;
using UrbanNoodle.Service;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using UrbanNoodle.Dto;
using UrbanNoodle.Services.Interface;
using UrbanNoodle.Services;
using UrbanNoodle.Utils;
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
            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

                    return new BadRequestObjectResult(new
                {
                    Status = 400,
                    Description = "Dữ liệu không hợp lệ",
                    Errors = errors
                });
                };
                });
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
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IFoodService, FoodService>();
            builder.Services.AddScoped<IDiningTable, DiningTableService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
