using Microsoft.EntityFrameworkCore;
using MovieCatalogAPI.Models;
using MovieCatalogAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//DataBase
var connection = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<MovieDBContext>(opts => opts.UseNpgsql(connection));

//Services
builder.Services.AddScoped<IMovieInfoService, MovieInfoService>();

var app = builder.Build();

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
