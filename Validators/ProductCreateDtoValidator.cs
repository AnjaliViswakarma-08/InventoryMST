using FluentValidation;
using InventoryMS.DTOs.Products;

namespace InventoryMS.Validators;

public sealed class ProductCreateDtoValidator : AbstractValidator<ProductCreateDto>
{
    public ProductCreateDtoValidator()
    {
        RuleFor(product => product.ProductName).NotEmpty().MaximumLength(100);
        RuleFor(product => product.Description).NotEmpty().MaximumLength(250);
        RuleFor(product => product.Price).GreaterThan(0);
        RuleFor(product => product.QuantityInStock).GreaterThanOrEqualTo(0);
        RuleFor(product => product.SupplierId).GreaterThan(0);
    }
}
