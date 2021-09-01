using FluentValidation;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using URLdata.Models;

namespace URLdata
{
    public class UrlRequestValidator: AbstractValidator<Url>
    {
        public UrlRequestValidator()
        {
            RuleFor(req => req)
                .Cascade(CascadeMode.Stop)
                .Must(UrlValidate).WithMessage("Must be  valid url");
        }

        private static bool UrlValidate(Url arg)
        {
            return arg.address.Length >= 7 && arg.address.StartsWith("www");
        }
    }
}