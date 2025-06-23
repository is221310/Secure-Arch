using SecurityArch;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Npgsql;

namespace SecureArchCore;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            DotNetEnv.Env.Load();
        }
        catch (FileNotFoundException)
        {
            //in container environment, .env file is not needed 
        }
        var builder = WebApplication.CreateBuilder(args);

        // === Umgebungsvariablen laden ===
        var blazorClientUrl = Environment.GetEnvironmentVariable("BLAZOR_APP_URL");
        var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
        var dbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(jwtSecret))
        {
            throw new Exception("JWT_SECRET_KEY Umgebungsvariable ist nicht gesetzt.");
        }

        if (string.IsNullOrWhiteSpace(dbConnectionString))
        {
            throw new Exception("DB_CONNECTION_STRING Umgebungsvariable ist nicht gesetzt.");
        }

        // === Datenbank-Konfiguration ===
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(dbConnectionString));

        builder.Services.AddHttpClient();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // === CORS Konfiguration ===
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowBlazorClient", policy =>
            {
                policy.WithOrigins(blazorClientUrl ?? "https://localhost:7255")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });
        // === JWT Authentifizierung ===
        builder.Services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var token = context.Request.Cookies["auth-token"];
                    Console.WriteLine("JWT Cookie: " + token);
                    if (!string.IsNullOrEmpty(token))
                    {
                        context.Token = token;
                    }
                    return Task.CompletedTask;
                }
            };

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = false,
                ValidIssuer = "https://wwww.SecureArch.at",
                ValidAudience = "https://www.S.at",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
            };
        });

        var app = builder.Build();

        // === HTTP Pipeline ===
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("AllowBlazorClient");
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}