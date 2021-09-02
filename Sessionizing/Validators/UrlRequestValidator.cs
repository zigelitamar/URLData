using System;
using System.Data;
using FluentValidation;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using URLdata.Models;

namespace URLdata
{
    public class UrlRequestValidator: AbstractValidator<Url>
    {
        public UrlRequestValidator()
        {

            RuleFor(req => req.address)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("No Url Inserted.")
                .Must(UrlValidate).WithMessage("Url must be valid.");
        }

        private static bool UrlValidate(string address)
        {
            return address.Length >= 7 && address.StartsWith("www");
        }
    }
}