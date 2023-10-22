using Microsoft.Extensions.Configuration;
using SaveJsonFileCURD.Models;
using SaveJsonFileCURD.Repository;
using System.Runtime;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.AddControllers();
// Configure a strongly-typed object
//builder.Services.Configure<User>(builder.Configuration.GetSection("DefaultUserPassword"));

builder.Services.Configure<User>(builder.Configuration.GetSection("MySettings"));
builder.Services.AddSingleton<IUserDataStore, JsonUserDataStore>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
