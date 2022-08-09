namespace Sample.Api.Controllers;

using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("[controller]")]
public class OrderController :
    ControllerBase
{
    readonly ILogger<OrderController> _logger;

    public OrderController(ILogger<OrderController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid id, [FromServices] IRequestClient<GetOrderStatus> client)
    {
        Response<OrderStatus, OrderNotFound> response = await client.GetResponse<OrderStatus, OrderNotFound>(new GetOrderStatus(id));

        return response.Is(out Response<OrderStatus>? order)
            ? Ok(order!.Message)
            : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Order order, [FromServices] IRequestClient<SubmitOrder> client)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // this is purely to allow the delay to be specified for demonstration purposes
        // otherwise, it is randomly assigned
        TimeSpan? fulfillmentDelay = default;
        if (Request.Headers.TryGetValue("Fulfillment-Delay", out var values) && int.TryParse(values.ToString(), out var delayInMinutes))
            fulfillmentDelay = TimeSpan.FromMinutes(delayInMinutes);

        var submitOrder = new SubmitOrder(order.OrderId, fulfillmentDelay);

        Response<OrderSubmissionAccepted> response = await client.GetResponse<OrderSubmissionAccepted>(submitOrder);

        return Ok(new { response.Message.OrderId });
    }
}