using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.VerifyOtp
{
    public class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
    {
        public VerifyOtpCommandValidator()
        {
            RuleFor(v => v.UserId).GreaterThan(0);
            RuleFor(v => v.OtpCode)
                .NotEmpty().WithMessage("OTP code is required.")
                .Length(4).WithMessage("OTP code must be 4 digits.") // Hoặc độ dài bạn đã generate
                .Matches("^[0-9]{4}$").WithMessage("OTP code must contain only digits."); // Đảm bảo là số
        }
    }
    }
