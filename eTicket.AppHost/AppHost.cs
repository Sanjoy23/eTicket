var builder = DistributedApplication.CreateBuilder(args);
var eventApi = builder.AddProject<Projects.Event_API>("apiservice-event");
var bookingApi = builder.AddProject<Projects.Booking_API>("apiservice-booking");
var paymentApi = builder.AddProject<Projects.ePayment_API>("apiservice-payment");

builder.Build().Run();
