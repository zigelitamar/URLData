using System;
using FluentValidation;
using URLdata.Models;

namespace URLdata
{
    public class VisitorIdRequestValidator : AbstractValidator<VisitorId>
    {
        public VisitorIdRequestValidator()
        {
            RuleFor(req => req.visitorId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("No visitor ID Inserted.")
                .GreaterThan(0).WithMessage("The Inserted visitor ID must b a positive number.");
        }
    }
}