using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class ValidationException : ApplicationException
    {
        public List<string> Errors { get; set; } = new List<string>();
        /// <summary>
        /// Từ điển chứa lỗi validation, nhóm theo tên thuộc tính.
        /// Key là tên thuộc tính, Value là mảng các thông báo lỗi cho thuộc tính đó.
        /// </summary>
        public IReadOnlyDictionary<string, string[]> ErrorsDic { get; }

        // Constructor mặc định
        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            ErrorsDic = new Dictionary<string, string[]>();
        }

        // Constructor nhận danh sách lỗi từ FluentValidation
        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this() // Gọi constructor mặc định trước
        {
            // Nhóm các lỗi theo tên thuộc tính
            ErrorsDic = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }
        public ValidationException(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
            {
                Errors.Add(error.ErrorMessage);
            }
        }
    }
}
