namespace Sample.Api.Tests;

using System.Net.Http.Json;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;


public class Submitting_an_order
{
    [Test]
    public async Task Should_have_the_submitted_status()
    {
        await using WebApplicationFactory<Program> application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.ConfigureServices(services => services.AddMassTransitTestHarness(x =>
            {
                x.AddHandler(async (SubmitOrder order) => new OrderSubmissionAccepted(order.OrderId));
            })));

        using var client = application.CreateClient();

        const string submitOrderUrl = "/Order";

        var orderId = NewId.NextGuid();

        var submitOrderResponse = await client.PostAsync(submitOrderUrl, JsonContent.Create(new Order { OrderId = orderId }));

        submitOrderResponse.EnsureSuccessStatusCode();
        var orderStatus = await submitOrderResponse.Content.ReadFromJsonAsync<OrderStatus>();

        Assert.That(orderStatus, Is.Not.Null);
        Assert.That(orderStatus!.OrderId, Is.EqualTo(orderId));
    }
}