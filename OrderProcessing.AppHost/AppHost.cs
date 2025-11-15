var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL - will automatically use Azure Database for PostgreSQL when deployed to Azure
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .PublishAsAzurePostgresFlexibleServer();

var orderDb = postgres.AddDatabase("orderdb");

// Add RabbitMQ - will run as Container App when deployed to Azure
var messaging = builder.AddRabbitMQ("messaging");

// Add API with PostgreSQL and RabbitMQ references
// WaitFor ensures the API doesn't start until dependencies are ready
var api = builder.AddProject<Projects.OrderProcessing_API>("orderprocessing-api")
    .WithReference(orderDb)
    .WithReference(messaging)
    .WaitFor(postgres)
    .WaitFor(messaging)
    .WithExternalHttpEndpoints();

// Add Worker with PostgreSQL and RabbitMQ references
// WaitFor ensures the Worker doesn't start until dependencies are ready
var worker = builder.AddProject<Projects.OrderProcessing_Worker>("orderprocessing-worker")
    .WithReference(orderDb)
    .WithReference(messaging)
    .WaitFor(postgres)
    .WaitFor(messaging);

builder.Build().Run();
