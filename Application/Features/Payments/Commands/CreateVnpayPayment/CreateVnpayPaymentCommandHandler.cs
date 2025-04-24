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

namespace Application.Features.Payments.Commands.CreateVnpayPayment
{

    public class CreateVnpayPaymentCommandHandler : IRequestHandler<CreateVnpayPaymentCommand, IResult<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IVnpayService _vnpayService;
        private readonly IHttpContextAccessor _httpContextAccessor; // Cần để truyền HttpContext vào VnpayService
        private readonly ILogger<CreateVnpayPaymentCommandHandler> _logger;

        public CreateVnpayPaymentCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IVnpayService vnpayService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateVnpayPaymentCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _vnpayService = vnpayService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<IResult<string>> Handle(CreateVnpayPaymentCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
            {
                return Result<string>.Unauthorized();
            }

            // Kiểm tra xem user đã premium chưa
            var user = await _unitOfWork.Users.GetByIdAsync(userId.Value, cancellationToken);
            if (user == null)
            {
                return Result<string>.Failure("User not found.", StatusCodes.Status404NotFound);
            }
            if (user.IsPremium)
            {
                return Result<string>.Failure("User is already a premium member.", StatusCodes.Status400BadRequest);
            }

            // 1. Tạo OrderId duy nhất (Ví dụ: dùng thời gian + UserID)
            // Cần đảm bảo OrderId là duy nhất trong hệ thống VNPAY của bạn
            var orderId = $"PremUpgr_{userId.Value}_{DateTime.UtcNow:yyyyMMddHHmmssfff}";

            // 2. Tạo và lưu bản ghi PaymentTransaction vào DB
            var transaction = PaymentTransaction.Create(
                userId.Value,
                orderId,
                request.Amount,
                request.OrderInfo,
                PaymentProvider.VNPAY // Đặt provider là Vnpay
            );

            try
            {
                await _unitOfWork.PaymentTransactions.AddAsync(transaction, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken); // Lưu transaction trước khi tạo URL
                _logger.LogInformation("Created pending VNPAY transaction {OrderId} for User {UserId}, Amount {Amount}", orderId, userId.Value, request.Amount);

                // 3. Lấy HttpContext hiện tại (quan trọng)
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    _logger.LogError("HttpContext is null. Cannot generate VNPAY URL.");
                    return Result<string>.Failure("Failed to access request context.", StatusCodes.Status500InternalServerError);
                }

                // 4. Tạo URL thanh toán VNPAY
                var paymentUrl = _vnpayService.GeneratePaymentUrl(
                    request.Amount,
                    transaction.OrderId, // Dùng OrderId đã lưu
                    transaction.OrderInfo,
                    httpContext
                );

                // 5. Trả về URL cho client
                return Result<string>.Success(paymentUrl, StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating VNPAY payment URL for User {UserId}, OrderInfo: {OrderInfo}", userId.Value, request.OrderInfo);
                return Result<string>.Failure($"An error occurred while creating payment URL: {ex.Message}", StatusCodes.Status500InternalServerError);
            }
        }
    }
    }
