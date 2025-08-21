using Library.Core.Middlewares;
using Library.Core.Serilog;
using Library.Database.Contexts.Public;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

const string connName = "Default";
const string jwtScheme = "Bearer";

var builder = WebApplication.CreateBuilder(args);

// 1) Services
AddCoreServices(builder);
AddSwaggerWithJwt(builder);
AddDb(builder);
AddLogging(builder);

// 2) Build
var app = builder.Build();

// 3) Pipeline
UseHttpPipeline(app);

app.Run();


// ------------------------ Clean helpers (same file) ------------------------

static void AddCoreServices(WebApplicationBuilder builder)
{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
}

static void AddSwaggerWithJwt(WebApplicationBuilder builder)
{
    builder.Services.AddSwaggerGen(o =>
    {
        o.SwaggerDoc("v1", new OpenApiInfo { Title = builder.Environment.ApplicationName, Version = "v1" });

        o.AddSecurityDefinition(jwtScheme, new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = jwtScheme,
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme."
        });

        o.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = jwtScheme }
                },
                Array.Empty<string>()
            }
        });
    });
}

static void AddDb(WebApplicationBuilder builder)
{
    var conn = builder.Configuration.GetConnectionString(connName);
    if (string.IsNullOrWhiteSpace(conn))
        throw new InvalidOperationException($"Missing connection string '{connName}'.");

    builder.Services.AddDbContext<PublicDbContext>(opt =>
    {
        opt.UseNpgsql(conn);
        // 若預設就好，拿掉下面這行
        // opt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    });
}

static void AddLogging(WebApplicationBuilder builder)
{
    // Library.Core.Serilog 擴充
    builder.Host.UseLibrarySerilog();
}

static void UseHttpPipeline(WebApplication app)
{
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

    // 自訂中介層（請確保已在 Library.Core.Middlewares 設好）
    app.UseMiddleware<RequestIdMiddleware>();

    app.UseAuthorization();
    app.MapControllers();
}