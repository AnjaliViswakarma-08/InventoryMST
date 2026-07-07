using FluentValidation;
using InventoryMS.DTOs.Suppliers;

namespace InventoryMS.Validators;

public sealed class SupplierUpdateDtoValidator : AbstractValidator<SupplierUpdateDto>
{
    public SupplierUpdateDtoValidator()
    {
        RuleFor(s => s.SupplierName).NotEmpty().MaximumLength(100);
        RuleFor(s => s.ContactPerson).NotEmpty().MaximumLength(100);
        RuleFor(s => s.Phone).NotEmpty().MaximumLength(20);
        RuleFor(s => s.Email).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(s => s.Address).NotEmpty().MaximumLength(200);
    }
}
