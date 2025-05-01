using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApiAfternoon.Middlewares
{
    public class CustomAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomAuthMiddleware> _logger;
        private readonly string _secretKey;

        public CustomAuthMiddleware(RequestDelegate next
            , ILogger<CustomAuthMiddleware> logger,
            IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _secretKey = configuration["Jwt:SecretKey"] ?? throw new ArgumentNullException("Jwt:Secret not found in configuration");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();// bearer ihsbgfdiohgkhuisedbhfklvs
            if (token != null)
            {
                var principal = ValidateToken(token);

                if (principal != null)
                {
                    var fullnameItem = principal.Claims.FirstOrDefault(c => c.Type == "Fullname");
                    var username = principal.Identity.Name;
                    context.User = principal;
                    context.Items["User"] = new { Name = username, Fullname = fullnameItem.Value };
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }
            }
            else
            {
                _logger.LogWarning("Authorization header missing");
            }

            await _next(context);
        }

        private ClaimsPrincipal ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Token validation failed");
                return null;
            }
        }
    }

    public static class CustomAuthMiddlewareExtentions
    {
        public static IApplicationBuilder UseCustomAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomAuthMiddleware>();
        }
    }
}
