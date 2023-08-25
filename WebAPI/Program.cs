using Autofac;
using Autofac.Extensions.DependencyInjection;
using Business.Abstract;
using Business.Concrete;
using Business.DependencyResolvers.Autofac;
using Core.Utilities.Security.Encryption;
using Core.Utilities.Security.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Core.Utilities.IoC;
using Core.DependencyResolvers;
using Core.Extensions;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();

        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

        builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
        {
            builder.RegisterModule(new AutofacBusinessModule());
        });

        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
                };
            });

        builder.Services.AddDependencyResolvers(new ICoreModule[] { new CoreModule() });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.ConfigureCustomExceptionMiddleware();

        app.UseCors(builder => builder.WithOrigins("http://localhost:4200").AllowAnyHeader());

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseAuthentication();

        app.MapControllers();

        app.Run();
    }
}
