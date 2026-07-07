using FluentValidation;
using InventoryMS.DTOs.Orders;

namespace InventoryMS.Validators;

public sealed class OrderCreateDtoValidator : AbstractValidator<OrderCreateDto>
{
    public OrderCreateDtoValidator()
    {
        RuleFor(order => order.UserId).GreaterThan(0);
        RuleFor(order => order.Items).NotEmpty();
        RuleForEach(order => order.Items).SetValidator(new OrderItemCreateDtoValidator());
    }
}
