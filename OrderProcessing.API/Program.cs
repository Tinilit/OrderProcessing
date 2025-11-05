using OrderProcessing.API.Services;
using OrderProcessing.Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add RabbitMQ client
builder.AddRabbitMQClient("messaging");

// Add services to the container.
builder.Services.AddControllers();

// Register application services
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

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
