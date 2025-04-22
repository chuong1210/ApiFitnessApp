using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class UnauthorizedException : ApplicationException
    {
        public int ErrorCode { get; } = StatusCodes.Status401Unauthorized;

        public UnauthorizedException(int errorCode) : base("Bạn không được phép truy cập tài nguyên này!")
        {
            ErrorCode = errorCode == StatusCodes.Status403Forbidden ? StatusCodes.Status403Forbidden : ErrorCode;
        }
    }
}
