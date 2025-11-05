using OrderProcessing.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

// Add RabbitMQ client
builder.AddRabbitMQClient("messaging");

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.Run();
