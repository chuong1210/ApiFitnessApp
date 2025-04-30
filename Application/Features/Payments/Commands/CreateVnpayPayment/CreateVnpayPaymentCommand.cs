using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Payments.Commands.CreateVnpayPayment
{
    public record CreateVnpayPaymentCommand(
        decimal Amount, // Số tiền cần thanh toán (ví dụ: giá gói Premium)
        string OrderInfo // Mô tả đơn hàng (ví dụ: "Nang cap tai khoan Premium")
    ) : IRequest<IResult<string>>; // Trả về URL thanh toán VNPAY


}
