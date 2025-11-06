var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin();

var orderDb = postgres.AddDatabase("orderdb");

// Add RabbitMQ
var messaging = builder.AddRabbitMQ("messaging");

// Add API with PostgreSQL and RabbitMQ references
var api = builder.AddProject<Projects.OrderProcessing_API>("orderprocessing-api")
    .WithReference(orderDb)
    .WithReference(messaging);

// Add Worker with PostgreSQL and RabbitMQ references
var worker = builder.AddProject<Projects.OrderProcessing_Worker>("orderprocessing-worker")
    .WithReference(orderDb)
    .WithReference(messaging);

builder.Build().Run();
