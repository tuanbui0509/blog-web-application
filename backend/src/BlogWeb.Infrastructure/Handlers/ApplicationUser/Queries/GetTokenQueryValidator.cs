using BlogWeb.Infrastructure.ApplicationUser.Queries;
using FluentValidation;

namespace BlogWeb.Infrastructure.Handlers.ApplicationUser.Queries
{
    public class GetTokenQueryValidator: AbstractValidator<GetTokenQuery>
    {
        public GetTokenQueryValidator()
        {
            RuleFor(v => v.Email)
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters.")
                .NotEmpty().WithMessage("Email is required.");

            RuleFor(v => v.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}