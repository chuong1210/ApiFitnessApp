using Application.Responses.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Payments.Commands.ProcessVnpayPayment
{
    // Command này nhận dữ liệu từ VNPAY callback/IPN
    public record ProcessVnpayPaymentCommand(
        IQueryCollection VnpayData // Chứa tất cả tham số vnp_
                                   // bool IsIpn = false // Thêm cờ nếu cần phân biệt IPN và Return URL
        ) : IRequest<IResult<string>>; // Trả về thông báo xử lý
}
