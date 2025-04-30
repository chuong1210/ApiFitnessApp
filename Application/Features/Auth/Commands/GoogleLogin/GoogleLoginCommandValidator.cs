using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.GoogleLogin
{
    public class GoogleLoginCommandValidator : AbstractValidator<GoogleLoginCommand>
    {
        public GoogleLoginCommandValidator()
        {
            RuleFor(v => v.IdToken)
                .NotEmpty().WithMessage("Google ID Token is required.");
            // Có thể thêm kiểm tra độ dài cơ bản nếu muốn, nhưng việc xác thực token sẽ làm việc chính
        }
    }
}
