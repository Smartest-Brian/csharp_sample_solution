using Library.Core.Logging;
using Library.Core.Middlewares;
using Library.Database.Contexts.Public;

using Microsoft.EntityFrameworkCore;

using Service.GraphQL.GraphQL;

namespace Service.GraphQL
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigBasic(builder);
            ConfigDatabase(builder);
            ConfigSerilog(builder);
            ConfigGraphql(builder);
            ConfigApp(builder);
        }

        private static void ConfigBasic(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddCors();
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

        private static void ConfigSerilog(WebApplicationBuilder builder)
        {
            builder.UseSerilogLogging();
        }

        private static void ConfigGraphql(WebApplicationBuilder builder)
        {
            builder.Services
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .AddMutationType<Mutation>()
                .AddSubscriptionType<Subscription>()
                .AddInMemorySubscriptions();
        }

        private static void ConfigApp(WebApplicationBuilder builder)
        {
            var app = builder.Build();

            app.UseMiddleware<RequestIdMiddleware>();

            app.UseWebSockets(); // WebSocket 必備
            app.MapGraphQL(); // 預設路徑 /graphql
            app.Run();
        }
    }
}
