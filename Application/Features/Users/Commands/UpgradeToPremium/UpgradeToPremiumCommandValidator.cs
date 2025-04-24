using Application.Common.Interfaces;
using Domain.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands.UpgradeToPremium
{
    public class UpgradeToPremiumCommandValidator : AbstractValidator<UpgradeToPremiumCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpgradeToPremiumCommandValidator(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;

            // Quy tắc: Người dùng phải tồn tại và chưa phải là Premium
            RuleFor(x => x) // Validate toàn bộ command object
                .MustAsync(UserExistsAndIsNotPremium)
                .WithMessage("User not found or is already a premium member.");

            // Thêm các quy tắc khác nếu cần, ví dụ:
            RuleFor(x => x).MustAsync(HaveVerifiedEmail).WithMessage("Please verify your email before upgrading.");
        }

        private async Task<bool> UserExistsAndIsNotPremium(UpgradeToPremiumCommand command, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
            {
                return false; // Không có user ID -> không hợp lệ
            }

            var user = await _unitOfWork.Users.GetByIdAsync(userId.Value, cancellationToken);

            // User phải tồn tại VÀ chưa phải là premium
            return user != null && !user.IsPremium;
        }

        private async Task<bool> HaveVerifiedEmail(UpgradeToPremiumCommand command, CancellationToken cancellationToken)
        {
            // Logic kiểm tra email đã verify chưa (ví dụ: đọc trường EmailVerified từ User entity)
            var userId = _currentUserService.UserId;
            if (!userId.HasValue) return false;
            var user = await _unitOfWork.Users.GetByIdAsync(userId.Value, cancellationToken);
            return user?.EmailVerified ?? false; // Giả sử có trường EmailVerified
        }
    }
    }
