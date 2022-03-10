using FluentValidation;
using MarketingBox.RegistrationApi.Models.Registration.Contracts;

namespace MarketingBox.RegistrationApi.Models.Validators
{
    public class RegistrationCreateValidations : AbstractValidator<RegistrationCreateRequest>
    {
        public RegistrationCreateValidations()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Ip).NotEmpty().Length(7, 40);
            RuleFor(x => x.CountryCode).NotEmpty().Length(2,3);
            RuleFor(x => x.CountryCodeType).NotNull();
            RuleFor(x => x.FirstName).Length(1, 50);
            RuleFor(x => x.LastName).Length(1, 50);
            RuleFor(x => x.Phone).NotEmpty();
            RuleFor(x => x.Password).NotEmpty().Length(6, 40);
            RuleFor(x => x.OfferId).NotNull().GreaterThan(0);
        }
    }
}