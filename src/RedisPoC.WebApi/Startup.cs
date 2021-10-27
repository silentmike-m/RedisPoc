namespace RedisPoC.WebApi
{
    using System;
    using System.Reflection;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using Serilog;
    using RedisPoC.Application;
    using RedisPoC.Application.Common;
    using RedisPoC.Infrastructure;
    using RedisPoC.Infrastructure.HealthCheck.Extensions;
    using RedisPoC.Infrastructure.HealthCheck.Models;
    using RedisPoC.WebApi.Events;
    using RedisPoC.WebApi.Extensions;
    using RedisPoC.WebApi.Filters;
    using RedisPoC.WebApi.Models;
    using RedisPoC.WebApi.Services;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddSerilog());

            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddInfrastructure(this.Configuration);
            services.AddApplication();

            services.AddHttpContextAccessor();
            services.AddSingleton<ICurrentRequestService, CurrentRequestService>();

            var jwtConfiguration = this.Configuration.GetSection("Jwt").Get<JwtOptions>();

            services.AddAuthentication(c =>
            {
                c.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                c.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                c.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtConfiguration.Issuer,
                    ValidAudience = jwtConfiguration.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SecurityKey)),
                    ClockSkew = TimeSpan.Zero,
                };
                options.Events = new JwtTokenEvents();
            });

            services.AddControllers(options =>
            {
                options.Filters.Add<SerilogLoggingActionFilter>();
                options.Filters.Add<ApiExceptionFilterAttribute>();
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.ConfigureSwaggerGen(c =>
            {
                c.CustomSchemaIds(s => s.FullName);
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RedisPoC.WebApi", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(async (context, next) =>
            {
                context.Request.PathBase = new PathString("/api");
                await next();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RedisPoC WebApi v1"));

            var healthCheckOptions = new HealthCheckOptions
            {
                ResponseWriter = HealthCheckResponseWriter,
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status200OK,
                },
            };
            app.UseHealthChecks("/health", healthCheckOptions);

            app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = EnrichDiagnosticContext;
            });
            app.UseRequestResponseLogging();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void EnrichDiagnosticContext(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            var request = httpContext.Request;

            diagnosticContext.Set("Host", request.Host);
            diagnosticContext.Set("Protocol", request.Protocol);
            diagnosticContext.Set("Scheme", request.Scheme);

            foreach (var (name, value) in request.Headers)
            {
                diagnosticContext.Set(name, value);
            }

            if (request.QueryString.HasValue)
            {
                diagnosticContext.Set("QueryString", request.QueryString.Value);
            }

            diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

            var endpoint = httpContext.GetEndpoint();

            if (endpoint is { })
            {
                diagnosticContext.Set("EndpointName", endpoint.DisplayName);
            }
        }

        private static async Task HealthCheckResponseWriter(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";

            var healthCheck = report.GetHealthCheck();

            var response = new BaseResponse<MainHealthCheck>
            {
                Response = healthCheck,
            };

            var jsonOptions = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                WriteIndented = true,
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        }
    }
}
