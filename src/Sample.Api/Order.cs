namespace Sample.Api;

using System.ComponentModel.DataAnnotations;


public class Order
{
    [Required]
    public Guid OrderId { get; init; }
}