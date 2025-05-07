using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.ValidationAttributes
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class AllowedValuesAttribute : ValidationAttribute
    {
        private readonly string[] _allowedValues;
        private readonly bool _ignoreCase;

        // Constructor nhận danh sách giá trị cho phép
        public AllowedValuesAttribute(bool ignoreCase = true, params string[] allowedValues)
            : base("The value for {0} is not allowed.") // {0} sẽ được thay bằng DisplayName
        {
            if (allowedValues == null || allowedValues.Length == 0)
            {
                throw new ArgumentNullException(nameof(allowedValues), "At least one allowed value must be specified.");
            }
            _allowedValues = allowedValues;
            _ignoreCase = ignoreCase;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Cho phép null, dùng [Required] nếu cần
            }

            if (value is not string stringValue)
            {
                // Chỉ áp dụng cho kiểu string trong ví dụ này
                return new ValidationResult($"The field {validationContext.DisplayName} must be a string to use AllowedValuesAttribute.");
            }

            var comparer = _ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

            if (!_allowedValues.Contains(stringValue, comparer))
            {
                // Tạo thông báo lỗi chi tiết hơn
                var allowedValuesString = string.Join(", ", _allowedValues.Select(v => $"'{v}'"));
                var errorMessage = $"The value '{stringValue}' for {validationContext.DisplayName} is not allowed. Allowed values are: {allowedValuesString}.";
                return new ValidationResult(errorMessage);
                // Hoặc dùng thông báo lỗi mặc định:
                // return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
    }
