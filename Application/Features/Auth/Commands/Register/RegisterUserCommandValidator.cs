using Domain.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.Register
{

    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        private readonly IUnitOfWork _unitOfWork; // Inject UnitOfWork

        public RegisterUserCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(v => v.Email)
                .NotEmpty().WithMessage("Email is required.")
                .MaximumLength(150).WithMessage("Email must not exceed 150 characters.")
                .EmailAddress().WithMessage("Email format is invalid.")
                // Kiểm tra email không trùng lặp (async)
                .MustAsync(BeUniqueEmail).WithMessage("The specified email address is already registered.");

            RuleFor(v => v.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                // Thêm các quy tắc phức tạp hơn nếu cần (ví dụ: chứa chữ hoa, chữ thường, số, ký tự đặc biệt)
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.").When(x => !string.IsNullOrEmpty(x.Password))
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.").When(x => !string.IsNullOrEmpty(x.Password))
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.").When(x => !string.IsNullOrEmpty(x.Password))
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one non-alphanumeric character.").When(x => !string.IsNullOrEmpty(x.Password));


            // Validate các trường nullable nếu chúng có giá trị
            RuleFor(v => v.HeightCm)
                .GreaterThan(0).When(v => v.HeightCm.HasValue).WithMessage("Height must be positive.");

            RuleFor(v => v.WeightKg)
                .GreaterThan(0).When(v => v.WeightKg.HasValue).WithMessage("Weight must be positive.");

            // Có thể thêm validation cho Gender, BirthDate nếu cần
            RuleFor(v => v.BirthDate)
               .LessThan(DateOnly.FromDateTime(DateTime.Today)).When(v => v.BirthDate.HasValue).WithMessage("Birth date must be in the past.");
        }

        /// <summary>
        /// Phương thức kiểm tra email có duy nhất không (sử dụng IUnitOfWork).
        /// </summary>
        private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
        {
            // Chỉ kiểm tra nếu email không rỗng (tránh lỗi khi check DB)
            if (string.IsNullOrEmpty(email)) return true;

            // Gọi phương thức kiểm tra tồn tại từ repository
            return !await _unitOfWork.Users.DoesEmailExistAsync(email, cancellationToken);
        }
    }
    }
