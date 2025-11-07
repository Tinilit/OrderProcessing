using OrderProcessing.Application.Extensions;
using OrderProcessing.DataAccess.Extensions;
using OrderProcessing.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

// Register infrastructure services (Database + Messaging)
builder.Services.AddOrderProcessingInfrastructure(builder);

// Register application services (includes message handler)
builder.Services.AddApplicationServices();

// Register the worker
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

// ? AUTO-APPLY DATABASE MIGRATIONS ON STARTUP
await host.InitializeDatabaseAsync();

await host.RunAsync();
