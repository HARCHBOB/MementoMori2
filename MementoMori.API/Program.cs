using MementoMori.API.Service;
using MementoMori.API.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using MementoMori.API.Interfaces;
using System.Collections.Concurrent;
using MementoMori.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("WebApiDatabase"))
    );

builder.Services.AddScoped<IDeckHelper, DeckHelper>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepo, AuthRepo>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<ISpacedRepetition, SpacedRepetition>();
builder.Services.AddControllers();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

app.UseCors("AllowReactApp");

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }