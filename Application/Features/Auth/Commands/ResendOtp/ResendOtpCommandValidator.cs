using Domain.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.ResendOtp
{
    public class ResendOtpCommandValidator : AbstractValidator<ResendOtpCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ResendOtpCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            RuleFor(v => v.UserId).GreaterThan(0);
            RuleFor(v => v.UserId)
                .MustAsync(UserExistsAndNotVerified)
                .WithMessage("Cannot resend OTP. User not found or already verified.");
        }

        private async Task<bool> UserExistsAndNotVerified(int userId, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            return user != null && !user.EmailVerified;
        }
    }
}
