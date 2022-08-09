using MassTransit;
using Sample;
using Sample.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilogConfiguration();

builder.Host.UseMassTransitConfiguration(x =>
{
    x.AddHandler(async (SubmitOrder order) => new OrderSubmissionAccepted(order.OrderId));
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();


public partial class Program
{
}