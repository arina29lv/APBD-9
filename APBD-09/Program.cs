using Microsoft.Data.SqlClient;
using APBD_09.Infrastructure.Repositories;
using APBD_09.Application.Services;
using APBD_09.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddTransient<SqlConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IWarehousePerository, WarehouseRepository>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
