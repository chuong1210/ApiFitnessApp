using Domain.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        // Inject IUserRepository hoặc IUnitOfWork
        private readonly IUserRepository _userRepository;
        // Hoặc: private readonly IUnitOfWork _unitOfWork;

        // Cập nhật constructor để nhận dependency
        public CreateUserCommandValidator(IUserRepository userRepository /* Hoặc IUnitOfWork unitOfWork */)
        {
            _userRepository = userRepository;
            // Hoặc: _unitOfWork = unitOfWork;

            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(v => v.Email)
                .MaximumLength(150).WithMessage("Email must not exceed 150 characters.")
                .EmailAddress().WithMessage("Email format is invalid.")
                // --- Kiểm tra trùng lặp bất đồng bộ ---
                .MustAsync(BeUniqueEmail).WithMessage("The specified email address already exists.")
                // Chỉ thực hiện kiểm tra định dạng và trùng lặp khi email không rỗng
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(v => v.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

            RuleFor(v => v.HeightCm)
                .GreaterThan(0).WithMessage("Height must be positive.")
                .When(x => x.HeightCm.HasValue);

            RuleFor(v => v.WeightKg)
                .GreaterThan(0).WithMessage("Weight must be positive.")
                .When(x => x.WeightKg.HasValue);
        }

        /// <summary>
        /// Phương thức helper bất đồng bộ để kiểm tra tính duy nhất của email.
        /// </summary>
        /// <param name="email">Email cần kiểm tra.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True nếu email là duy nhất (chưa tồn tại), False nếu email đã tồn tại.</returns>
        private async Task<bool> BeUniqueEmail(string? email, CancellationToken cancellationToken)
        {
            // Nếu email rỗng hoặc null, coi như nó hợp lệ (không cần kiểm tra trùng)
            // vì rule .When() bên trên đã xử lý trường hợp này rồi, nhưng thêm ở đây để rõ ràng.
            if (string.IsNullOrEmpty(email))
            {
                return true;
            }

            // Gọi phương thức từ repository để kiểm tra
            // Dùng _userRepository hoặc _unitOfWork.Users tùy theo cách bạn inject
            bool emailExists = await _userRepository.DoesEmailExistAsync(email, cancellationToken);
            // Hoặc: bool emailExists = await _unitOfWork.Users.DoesEmailExistAsync(email, cancellationToken);

            // MustAsync cần trả về true nếu validation PASS (tức là email KHÔNG tồn tại)
            return !emailExists;
        }
    }

}