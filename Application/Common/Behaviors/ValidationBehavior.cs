using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Behaviors
{

    /// <summary>
    /// MediatR Pipeline Behavior để tự động thực thi validation bằng FluentValidation
    /// cho các Command và Query trước khi chúng được xử lý bởi Handler.
    /// </summary>
    /// <typeparam name="TRequest">Kiểu của Request (Command hoặc Query).</typeparam>
    /// <typeparam name="TResponse">Kiểu của Response.</typeparam>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        // Ràng buộc TRequest phải là một IRequest<TResponse> để đảm bảo nó là MediatR request
        where TRequest : IRequest<TResponse>
    {
        // Inject danh sách tất cả các Validators cho kiểu TRequest
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger; // Optional for logging

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehavior<TRequest, TResponse>> logger)
        {
            _validators = validators;
            _logger = logger; // Inject logger nếu bạn muốn ghi log quá trình validation
        }

        /// <summary>
        /// Xử lý request trong pipeline.
        /// </summary>
        /// <param name="request">Request đang được xử lý.</param>
        /// <param name="next">Delegate để gọi bước tiếp theo trong pipeline (thường là Handler).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Kết quả trả về từ Handler (nếu validation thành công).</returns>
        /// <exception cref="ValidationException">Ném ra nếu có lỗi validation.</exception>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Kiểm tra xem có validator nào được đăng ký cho kiểu request này không
            if (!_validators.Any())
            {
                // Nếu không có validator, bỏ qua và chuyển sang handler tiếp theo
                return await next();
            }

            // Tạo ngữ cảnh validation
            var context = new ValidationContext<TRequest>(request);

            _logger.LogTrace("Validating request of type {RequestType}", typeof(TRequest).Name);

            // Thực thi tất cả các validator tìm thấy một cách bất đồng bộ
            // và thu thập kết quả (bao gồm cả lỗi validation)
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

            // Lọc ra các lỗi từ tất cả các kết quả validation
            var failures = validationResults
                .SelectMany(r => r.Errors) // Lấy tất cả các lỗi từ các validator
                .Where(f => f != null)     // Đảm bảo không có lỗi null (dù hiếm)
                .ToList();

            // Nếu có bất kỳ lỗi nào được tìm thấy
            if (failures.Any())
            {
                // Ghi log các lỗi validation
                _logger.LogWarning("Validation failed for {RequestType}. Errors: {@ValidationErrors}",
                                   typeof(TRequest).Name,
                                   failures.Select(f => new { f.PropertyName, f.ErrorMessage }));

                // Ném ra một Exception chứa các lỗi validation
                // Handler sẽ không được gọi
                throw new ValidationException(failures); // Sử dụng ValidationException tùy chỉnh của bạn
                                                         // Hoặc dùng ValidationException của FluentValidation:
                                                         // throw new FluentValidation.ValidationException(failures);
            }

            // Nếu không có lỗi, ghi log thành công (tùy chọn) và tiếp tục pipeline
            _logger.LogTrace("Validation successful for {RequestType}", typeof(TRequest).Name);
            return await next(); // Gọi Handler hoặc behavior tiếp theo
        }
    }
}
