# OrderProcessing

A distributed order processing system built with .NET 8 and .NET Aspire, demonstrating asynchronous message-based architecture with RabbitMQ and PostgreSQL.

## How to Run

### Prerequisites
- .NET 8 SDK
- Docker Desktop

### Running the Application

```bash
dotnet run --project OrderProcessing.AppHost
```
This will start PostgreSQL, RabbitMQ, and launch the API and Worker services. Access the Aspire Dashboard (typically at https://localhost:17251) to view service endpoints.


## Design Decisions and Trade-offs

### Asynchronous Processing with Message Queue
- **Decision**: API publishes to RabbitMQ; Worker persists to database
- **Trade-off**: Better response times and scalability vs. eventual consistency

### Repository Pattern
- **Decision**: Abstract data access through `IOrderRepository`
- **Trade-off**: Improved testability vs. additional abstraction layer

### Layered Architecture
- **Decision**: Separate projects by layer (API, Worker, Application, DataAccess, Core)
- **Trade-off**: Clear separation and simplicity vs. increased number of projects

### .NET Aspire for Orchestration
- **Decision**: Use Aspire instead of Docker Compose
- **Trade-off**: Integrated dashboard and telemetry vs. additional dependency

### Tests
- **Decision**: Unit tests for Application and DataAccess layers
- **Trade-off**: Ensures correctness of business logic and data access vs. increased development time


P.S.
I'll be glade to discuss any of these decisions further or provide additional details as needed!