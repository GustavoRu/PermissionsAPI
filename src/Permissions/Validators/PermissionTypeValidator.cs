using FluentValidation;
using BackendApi.Permissions.DTOs;

namespace BackendApi.Permissions.Validators
{
    public class PermissionTypeValidator : AbstractValidator<PermissionTypeDto>
    {
        public PermissionTypeValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(100).WithMessage("Description cannot exceed 100 characters");
        }
    }
} 