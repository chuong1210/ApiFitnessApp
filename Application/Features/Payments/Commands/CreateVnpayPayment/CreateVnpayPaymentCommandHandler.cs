using Application.Common.Interfaces;
using Application.Responses.Interfaces;
using Application.Responses;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNPay.NetCore;
using Microsoft.Extensions.Configuration;
namespace Application.Features.Payments.Commands.CreateVnpayPayment
{


    public class CreateVnpayPaymentCommandHandler : IRequestHandler<CreateVnpayPaymentCommand, IResult<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IVNPayService _vnpayService; // Inject service của thư viện
        private readonly IConfiguration _configuration;
        private readonly ILogger<CreateVnpayPaymentCommandHandler> _logger;

        public CreateVnpayPaymentCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IVNPayService vnpayService, // Inject service của thư viện
            IConfiguration configuration,
            ILogger<CreateVnpayPaymentCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _vnpayService = vnpayService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IResult<string>> Handle(CreateVnpayPaymentCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue) return Result<string>.Unauthorized();

            var user = await _unitOfWork.Users.GetByIdAsync(userId.Value, cancellationToken);
            if (user == null) return Result<string>.Failure("User not found.", StatusCodes.Status404NotFound);
            if (user.IsPremium) return Result<string>.Failure("User is already a premium member.", StatusCodes.Status400BadRequest);

            // --- SỬA LOGIC Ở ĐÂY ---
            // 1. Lấy giá premium từ config
            var premiumPrice = _configuration.GetValue<decimal>("Subscription:PremiumPrice", 50000); // Mặc định 50,000 VND

            // 2. Tạo đối tượng VNPayRequest
            var vnpayRequest = new VNPayRequest
            {
                RequestCode = Guid.NewGuid().ToString(), // Mã duy nhất cho request này
                OrderCode = $"PremUpgr_{userId.Value}_{DateTime.UtcNow:yyyyMMddHHmmss}", // Mã đơn hàng duy nhất
                Amount = premiumPrice, // Số tiền
                //OrderDescription = request.OrderInfo ?? $"Nang cap tai khoan Premium cho user {userId.Value}",
                //CreatedDate = DateTime.UtcNow
            };

            // 3. Tạo và lưu transaction vào DB
            var transaction = PaymentTransaction.Create(
                userId.Value, vnpayRequest.OrderCode, vnpayRequest.Amount,
                vnpayRequest.Data.ToString(), PaymentProvider.VNPAY);
            await _unitOfWork.PaymentTransactions.AddAsync(transaction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created pending VNPAY transaction {OrderId} for User {UserId}", transaction.OrderId, userId.Value);

            // 4. Tạo URL thanh toán bằng service của thư viện
            // URL redirect về frontend để hiển thị kết quả
            var frontendReturnUrl = _configuration["Vnpay:FrontendReturnUrl"]
                ?? "https://www.figma.com/design/tHV4FBgQS7sRP0hlFwsKAY/Fitnest---Fitness-App-UI-Kit-by-Pixel-True?node-id=801-3421";

            string paymentUrl = await _vnpayService.CreatePaymentLink(
                "PremiumUpgrade", // Chỉ định Processor sẽ xử lý callback
                vnpayRequest,
                frontendReturnUrl // URL để redirect người dùng sau khi thanh toán
            );

            _logger.LogInformation("Generated VNPAY payment URL: {PaymentUrl}", paymentUrl);

            return Result<string>.Success(paymentUrl, StatusCodes.Status200OK);
        }
    }

}