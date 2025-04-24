using Application.Responses.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using Application.Features.Payments.Commands.ProcessVnpayPayment;
namespace FitnessApp.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class VnpayController : ControllerBase
    {
        private readonly ISender _mediator;

        public VnpayController(ISender mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// VNPAY Return URL endpoint. Handles the callback after payment attempt.
        /// </summary>
        /// <returns>Redirects to frontend confirmation page or shows status.</returns>
        [HttpGet("callback")] // Giống ReturnUrl đã cấu hình
        [ProducesResponseType(StatusCodes.Status302Found)] // Thường sẽ Redirect
        [ProducesResponseType(typeof(IResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IResult<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PaymentCallback()
        {
            // Lấy toàn bộ query parameters từ request
            var vnpayData = HttpContext.Request.Query;

            var command = new ProcessVnpayPaymentCommand(vnpayData);
            var result = await _mediator.Send(command);

            // --- Xử lý kết quả và Redirect người dùng ---
            // Nên redirect về một trang trên frontend của bạn để hiển thị kết quả
            // Truyền trạng thái thành công/thất bại qua query string của URL redirect

            string frontendRedirectUrl = "https://your-frontend.com/payment/result"; // URL trang kết quả frontend

            if (result.Succeeded)
            {
                // Thêm OrderId và status vào URL redirect
                var orderId = vnpayData.FirstOrDefault(q => q.Key == "vnp_TxnRef").Value.ToString();
                var successUrl = $"{frontendRedirectUrl}?status=success&orderId={orderId}&message={HttpUtility.UrlEncode(result.Data)}"; // result.Data chứa thông báo
                return Redirect(successUrl); // HTTP 302 Found
            }
            else
            {
                // Thêm OrderId và thông báo lỗi vào URL redirect
                var orderId = vnpayData.FirstOrDefault(q => q.Key == "vnp_TxnRef").Value.ToString();
                var errorMessage = result.Messages.FirstOrDefault() ?? "Payment processing failed.";
                var failureUrl = $"{frontendRedirectUrl}?status=failed&orderId={orderId}&message={HttpUtility.UrlEncode(errorMessage)}";
                return Redirect(failureUrl);
            }

            // Hoặc trả về JSON nếu không muốn redirect (ít phổ biến cho return url)
            // return StatusCode(result.Code, result);
        }

        // --- (Tùy chọn nhưng khuyến nghị) Endpoint xử lý IPN ---
        // [HttpPost("ipn")]
        // public async Task<IActionResult> PaymentIpn()
        // {
        //     // Logic tương tự callback nhưng đọc Query và không redirect
        //     // var vnpayData = HttpContext.Request.Query;
        //     // var command = new ProcessVnpayPaymentCommand(vnpayData, isIpn: true); // Thêm cờ phân biệt IPN
        //     // var result = await _mediator.Send(command);
        //
        //     // Phản hồi lại cho VNPAY theo đúng định dạng yêu cầu
        //     // Xem tài liệu VNPAY để biết cách phản hồi IPN (thường là JSON với RspCode và Message)
        //     // if (result.Succeeded) return Ok(new { RspCode = "00", Message = "Confirm Success" });
        //     // else return Ok(new { RspCode = "99", Message = "Confirm Failed" }); // Hoặc mã lỗi khác
        // }
    }
    }
