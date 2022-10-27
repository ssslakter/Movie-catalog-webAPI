using Microsoft.EntityFrameworkCore;
using MovieCatalogAPI.Models;
using MovieCatalogAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MovieCatalogAPI.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//JWT auth
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        // указывает, будет ли валидироваться издатель при валидации токена
        ValidateIssuer = true,
        // строка, представляющая издателя
        ValidIssuer = JwtConfigurations.Issuer,
        // будет ли валидироваться потребитель токена
        ValidateAudience = true,
        // установка потребителя токена
        ValidAudience = JwtConfigurations.Audience,
        // будет ли валидироваться время существования
        ValidateLifetime = true,
        // установка ключа безопасности
        IssuerSigningKey = JwtConfigurations.GetSymmetricSecurityKey(),
        // валидация ключа безопасности
        ValidateIssuerSigningKey = true,

    };
});

//DataBase
var connection = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<MovieDBContext>(opts => opts.UseNpgsql(connection));
//Https client
builder.Services.AddHttpClient();
//Services
builder.Services.AddScoped<IMovieInfoService, MovieInfoService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMovieDataService,MovieDataService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IMovieConverterService, MovieConverterService>();
builder.Services.AddScoped<IFavoriteMoviesService,FavoriteMoviesService>();
builder.Logging.AddConsole();
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
