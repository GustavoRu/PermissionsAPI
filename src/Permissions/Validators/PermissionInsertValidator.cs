using FluentValidation;
using BackendApi.Permissions.DTOs;

namespace BackendApi.Permissions.Validators
{
    public class PermissionInsertValidator : AbstractValidator<PermissionInsertDto>
    {
        public PermissionInsertValidator()
        {
            RuleFor(x => x.EmployeeName)
                .NotEmpty().WithMessage("Employee name is required")
                .MaximumLength(100).WithMessage("Employee name cannot exceed 100 characters");

            RuleFor(x => x.EmployeeSurname)
                .NotEmpty().WithMessage("Employee surname is required")
                .MaximumLength(100).WithMessage("Employee surname cannot exceed 100 characters");

            RuleFor(x => x.PermissionTypeId)
                .NotEmpty().WithMessage("Permission type is required")
                .GreaterThan(0).WithMessage("Invalid permission type");

            // RuleFor(x => x.PermissionDate)
            //     .NotEmpty().WithMessage("Permission date is required")
            //     .Must(date => date != default).WithMessage("Invalid permission date");
        }
    }
} 