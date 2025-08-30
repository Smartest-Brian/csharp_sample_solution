using Library.Core.Logging;
using Library.Core.Middlewares;
using Library.Core.Time;
using Library.Database.Contexts.Public;

using Microsoft.EntityFrameworkCore;

namespace Service.Auth
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
        }

        private static void ConfigSwagger(WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }

        private static void ConfigDatabase(WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("PostgreSql");
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
