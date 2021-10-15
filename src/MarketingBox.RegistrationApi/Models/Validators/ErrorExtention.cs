using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace MarketingBox.RegistrationApi.Models.Validators
{
    public class ErrorMessageContract
    {
        public string ErrorMessage { get; set; }

        public static ErrorMessageContract Create(ValidationFailure src)
        {
            return new ErrorMessageContract
            {
                ErrorMessage = src.ErrorMessage
            };
        }
    }
    
    public static class FluentErrorExtension
    {
        public static IEnumerable<ErrorMessageContract> GetErrors(this ValidationResult result)
        {
            return result.Errors.Select(ErrorMessageContract.Create);
        }
    }
}