using Application.Features.Payments.Commands.CreateVnpayPayment;
using Application.Responses.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitnessApp.Contracts;
using FitnessApp.Contracts.Requests;
namespace FitnessApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly ISender _mediator;

        public PaymentsController(ISender mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a VNPAY payment URL for upgrading the current user to premium.
        /// </summary>
        /// <param name="request">Payment details (Amount, OrderInfo).</param>
        /// <returns>The VNPAY payment URL.</returns>
        [HttpPost("create-vnpay-premium-url")]
        [Authorize] // Yêu cầu đăng nhập
        [ProducesResponseType(typeof(IResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IResult<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(IResult<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateVnpayPremiumUrl([FromBody] CreateVnpayPaymentRequest request) // Tạo DTO cho request body
        {
            // Lấy giá Premium từ cấu hình hoặc DB thay vì để client gửi lên trực tiếp
            // Ví dụ: decimal premiumPrice = _configuration.GetValue<decimal>("Subscription:PremiumPrice");
            decimal premiumPrice = 50000; // Ví dụ: 50,000 VND

            var command = new CreateVnpayPaymentCommand(premiumPrice, request.OrderInfo ?? "Upgrade to Premium");
            var result = await _mediator.Send(command);
            return StatusCode(result.Code, result);
        }
    }

}
