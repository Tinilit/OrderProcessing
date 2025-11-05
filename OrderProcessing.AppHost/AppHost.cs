var builder = DistributedApplication.CreateBuilder(args);

// Add RabbitMQ
var messaging = builder.AddRabbitMQ("messaging");

// Add API with RabbitMQ reference
var api = builder.AddProject<Projects.OrderProcessing_API>("orderprocessing-api")
    .WithReference(messaging);

// Add Worker with RabbitMQ reference
var worker = builder.AddProject<Projects.OrderProcessing_Worker>("orderprocessing-worker")
    .WithReference(messaging);

builder.Build().Run();
