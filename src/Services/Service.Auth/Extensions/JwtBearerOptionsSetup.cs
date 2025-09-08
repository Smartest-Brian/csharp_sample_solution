using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.Auth.Options;

namespace Service.Auth.Extensions
{
    internal sealed class JwtBearerOptionsSetup(IOptions<JwtOptions> jwtOptions) : IConfigureOptions<JwtBearerOptions>
    {
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;

        public void Configure(JwtBearerOptions options)
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key))
            };
        }
    }
}
