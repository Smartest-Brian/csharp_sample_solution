using System.Text;

using Library.ApiClient.Services.Auth;
using Library.Core.Logging;
using Library.Core.Middlewares;
using Library.Core.Time;
using Library.Database.Contexts.Public;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Refit;

using Service.WebAPI.Services.Country;

namespace Service.WebAPI
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            ConfigBasic(builder);
            ConfigService(builder);
            ConfigSwagger(builder);
            ConfigDatabase(builder);
            ConfigSerilog(builder);
            ConfigApp(builder);
        }

        private static void ConfigBasic(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddCors();
        }

        private static void ConfigService(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICountryService, CountryService>();
            builder.Services.AddTimezoneService();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            });

            builder.Services
                .AddRefitClient<IAuthApi>()
                .ConfigureHttpClient(c =>
                    c.BaseAddress = new Uri(builder.Configuration["AuthApi:BaseUrl"]!));
        }

        private static void ConfigSwagger(WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Auth API",
                    Version = "v1"
                });

                // 加入 JWT 驗證設定
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "請輸入 JWT Token，格式: Bearer {your token}"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

        private static void ConfigDatabase(WebApplicationBuilder builder)
        {
            string? connectionString = builder.Configuration.GetConnectionString("PostgreSql");
            if (string.IsNullOrWhiteSpace(connectionString)) throw new InvalidOperationException($"Connection String Not Found.");

            builder.Services.AddDbContext<PublicDbContext>(opt =>
            {
                opt.UseNpgsql(connectionString);
                opt.EnableSensitiveDataLogging();
            });
        }

        private static void ConfigSerilog(WebApplicationBuilder builder) => builder.UseSerilogLogging();

        private static void ConfigApp(WebApplicationBuilder builder)
        {
            WebApplication app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<RequestIdMiddleware>();
            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseCors("AllowSpecificOrigin");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
