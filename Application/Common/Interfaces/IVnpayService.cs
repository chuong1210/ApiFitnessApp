using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IVnpayService
    {
        string GeneratePaymentUrl(decimal amount, string orderId, string orderInfo, HttpContext httpContext);
        bool ValidateSignature(IQueryCollection vnpayData, string inputHash);
    }
}
