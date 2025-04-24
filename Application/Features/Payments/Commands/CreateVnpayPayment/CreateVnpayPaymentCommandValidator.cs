using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Payments.Commands.CreateVnpayPayment
{
    public class CreateVnpayPaymentCommandValidator : AbstractValidator<CreateVnpayPaymentCommand>
    {
        public CreateVnpayPaymentCommandValidator()
        {
            RuleFor(v => v.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");
            RuleFor(v => v.OrderInfo)
               .NotEmpty().WithMessage("Order information is required.")
               .MaximumLength(255);
        }
    }
}
