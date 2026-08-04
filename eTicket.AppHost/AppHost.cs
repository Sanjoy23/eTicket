var builder = DistributedApplication.CreateBuilder(args);
builder.AddProject<Projects.Event_API>("apiservice-event");
builder.AddProject<Projects.Booking_API>("apiservice-booking");
builder.AddProject<Projects.ePayment_API>("apiservice-payment");
builder.AddProject<Projects.Identity_API>("apiservice-identity");
builder.AddProject<Projects.eTicket_ApiGateway>("eticket-apigateway");

await builder.Build().RunAsync();
