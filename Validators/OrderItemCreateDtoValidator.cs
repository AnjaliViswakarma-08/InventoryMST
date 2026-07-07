using FluentValidation;
using InventoryMS.DTOs.Orders;

namespace InventoryMS.Validators;

public sealed class OrderItemCreateDtoValidator : AbstractValidator<OrderItemCreateDto>
{
    public OrderItemCreateDtoValidator()
    {
        RuleFor(item => item.ProductId).GreaterThan(0);
        RuleFor(item => item.Quantity).GreaterThan(0);
    }
}
