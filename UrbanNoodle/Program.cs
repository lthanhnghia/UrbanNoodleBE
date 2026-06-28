
using System.Text;
using System.Text.Json;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SemanticKernel;
using UrbanNoodle.ApplicationContext;
using UrbanNoodle.Dto;
using UrbanNoodle.Middleware;
using UrbanNoodle.Repository;
using UrbanNoodle.Repository.Interface;
using UrbanNoodle.Service;
using UrbanNoodle.Service.Interface;
using UrbanNoodle.Services;
using UrbanNoodle.Services.Interface;
namespace UrbanNoodle
{
    public class Program
    {
        public static void Main(string[] args)
        {

            Env.Load();
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            var builder = WebApplication.CreateBuilder(args);
            string modelId = "gemini-2.5-flash";
            string apiKey = Environment.GetEnvironmentVariable("API_KEY");

            // Add services to the container.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Environment.GetEnvironmentVariable("DEFAULT_CONNECTION"),
                npgsqlOptions => npgsqlOptions.UseVector()
                ));
            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value.Errors.Select(e =>
                        {
                            if (e.ErrorMessage.Contains("The value '' is invalid."))
                            {
                                return x.Key switch
                                {
                                    "Price" => "Giá món ăn không được để trống.",
                                    "CategoryId" => "Danh mục không được để trống.",
                                    _ => "Dữ liệu không hợp lệ."
                                };
                            }

                            return e.ErrorMessage;
                        }).ToArray()
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

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!)
            )
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                // chặn response mặc định
                context.HandleResponse();

                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var response = new ApiResponse(401, "Đăng nhập thất bại");

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(response)
                );
            },

            OnForbidden = async context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";

                var response = new ApiResponse(403, "Bạn không có quyền sử dụng tài nguyên này");

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(response)
                );
            }
        };
    });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            //builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IFoodService, FoodService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            //builder.Services.AddScoped<IDashboardService, DashboardService>();

            builder.Services.AddScoped<IAlService, AIService>();
            builder.Services.AddScoped<IKnowledgeChunksRepository, KnowledgeChunksRepository>();
            builder.Services.AddMemoryCache();
            builder.Services.AddHttpClient();
            builder.Logging.AddFilter("System.Net.Http", LogLevel.Trace);
            builder.Logging.AddFilter("Microsoft.SemanticKernel", LogLevel.Trace);
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();
            builder.Services.AddGoogleAIGeminiChatCompletion(
                modelId: modelId,
                apiKey: apiKey
                );



            builder.Services.AddGoogleAIEmbeddingGenerator(
                modelId: "gemini-embedding-001",
                apiKey: apiKey
                );
            builder.Services.AddScoped<ToolAlService>();

            builder.Services.AddScoped<KernelPluginCollection>((serviceProvider) =>
               [
                 KernelPluginFactory.CreateFromObject(serviceProvider.GetRequiredService<ToolAlService>())
               ]
            );

            builder.Services.AddTransient((serviceProvider) =>
            {
                KernelPluginCollection pluginCollection = serviceProvider.GetRequiredService<KernelPluginCollection>();
                return new Kernel(serviceProvider, pluginCollection);
            });
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20); // Sau 20 phút khách không tương tác thì xóa Session
                options.Cookie.HttpOnly = true;                // Bảo mật Cookie chống tấn công XSS
                options.Cookie.IsEssential = true;             // Bắt buộc phải có để chạy luồng Session
            });
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();
            app.UseSession();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
