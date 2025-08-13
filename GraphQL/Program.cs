using GraphQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using GraphQL.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddSubscriptionType<Subscription>()
    .AddInMemorySubscriptions(); // 換成 AddRedisSubscriptions() … 亦可

var app = builder.Build();
app.UseMiddleware<LogIdMiddleware>();
app.UseWebSockets(); // WebSocket 必備
app.MapGraphQL(); // 預設路徑 /graphql
app.Run();
