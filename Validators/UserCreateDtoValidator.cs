using FluentValidation;
using InventoryMS.DTOs.Users;
using InventoryMS.Helpers;

namespace InventoryMS.Validators;

public sealed class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateDtoValidator()
    {
        RuleFor(user => user.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(user => user.LastName).NotEmpty().MaximumLength(50);
        RuleFor(user => user.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(user => user.Phone).NotEmpty().MaximumLength(20);
        RuleFor(user => user.Address).NotEmpty().MaximumLength(250);
        RuleFor(user => user.Gender).NotEmpty().MaximumLength(20);
        RuleFor(user => user.Age).InclusiveBetween(18, 120);
        RuleFor(user => user.Password).NotEmpty().MinimumLength(6).MaximumLength(100);
        RuleFor(user => user.Role).NotEmpty().Must(role =>
            string.Equals(role, RoleName.Owner, System.StringComparison.OrdinalIgnoreCase) ||
            string.Equals(role, RoleName.HR, System.StringComparison.OrdinalIgnoreCase) ||
            string.Equals(role, RoleName.AdminStaff, System.StringComparison.OrdinalIgnoreCase) ||
            string.Equals(role, RoleName.ViewerStaff, System.StringComparison.OrdinalIgnoreCase) ||
            string.Equals(role, RoleName.EditorStaff, System.StringComparison.OrdinalIgnoreCase))
            .WithMessage("Role must be one of: Owner, HR, AdminStaff, ViewerStaff, EditorStaff.");
    }
}
