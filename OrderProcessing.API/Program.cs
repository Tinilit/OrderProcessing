using OrderProcessing.Application.Extensions;
using OrderProcessing.DataAccess.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container
builder.Services.AddControllers();

// Register infrastructure services (Database + Messaging)
builder.Services.AddOrderProcessingInfrastructure(builder);

// Register application services
builder.Services.AddApplicationServices();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ? AUTO-APPLY DATABASE MIGRATIONS ON STARTUP
await app.InitializeDatabaseAsync();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
