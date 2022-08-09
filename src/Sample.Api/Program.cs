using MassTransit;
using Sample.Contracts;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMassTransit(x =>
{
    x.AddHandler(async (SubmitOrder order) => new OrderSubmissionAccepted(order.OrderId));

    x.SetKebabCaseEndpointNameFormatter();

    x.UsingAzureServiceBus((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("ServiceBus"));

        cfg.Send<SubmitOrder>(s => s.UseSessionIdFormatter(c => c.Message.OrderId.ToString("D")));
        
        cfg.ConfigureEndpoints(context);
    });
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();