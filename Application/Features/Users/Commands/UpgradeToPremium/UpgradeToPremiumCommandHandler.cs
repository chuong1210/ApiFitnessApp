﻿using Application.Common.Interfaces;
using Application.Responses.Interfaces;
using Application.Responses;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Application.Common.Dtos;
using Domain.Enums;
namespace Application.Features.Users.Commands.UpgradeToPremium
{
    public class UpgradeToPremiumCommandHandler : IRequestHandler<UpgradeToPremiumCommand, IResult<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UpgradeToPremiumCommandHandler> _logger;
        private readonly IBackgroundJobClient _backgroundJobClient; // Inject Hangfire

         private readonly IVnpayService _paymentService; // Inject dịch vụ thanh toán thật

        public UpgradeToPremiumCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ILogger<UpgradeToPremiumCommandHandler> logger
            , IVnpayService paymentService, IBackgroundJobClient backgroundJobClient // Inject Hangfire
                ) // Inject dịch vụ thanh toán thật
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
             _paymentService = paymentService;
            _backgroundJobClient = backgroundJobClient;
        }

        //public async Task<IResult<string>> Handle(UpgradeToPremiumCommand request, CancellationToken cancellationToken)
        //{
        //    // 1. Lấy User ID từ CurrentUserService (đã được xác thực bởi [Authorize])
        //    var userId = _currentUserService.UserId;
        //    if (!userId.HasValue)
        //    {
        //        // Trường hợp này không nên xảy ra nếu [Authorize] hoạt động đúng
        //        _logger.LogWarning("UpgradeToPremium: Unauthorized attempt detected.");
        //        return Result<string>.Unauthorized();
        //    }

        //    // 2. Lấy thông tin User từ DB
        //    var user = await _unitOfWork.Users.GetByIdAsync(userId.Value, cancellationToken);
        //    if (user == null)
        //    {
        //        // User ID trong token hợp lệ nhưng không tìm thấy user trong DB? Lạ.
        //        _logger.LogError("UpgradeToPremium: User with ID {UserId} from token not found in database.", userId.Value);
        //        // Trả về lỗi 404 thay vì 401 vì user đã được authorize trước đó
        //        return Result<string>.Failure($"User not found.", StatusCodes.Status404NotFound);
        //    }

        //    // 3. Kiểm tra nếu user đã là Premium (Validator đã kiểm tra, nhưng kiểm tra lại cho chắc)
        //    if (user.IsPremium)
        //    {
        //        _logger.LogInformation("UpgradeToPremium: User {UserId} is already premium.", userId.Value);
        //        return Result<string>.Failure("Your account is already premium.", StatusCodes.Status400BadRequest);
        //    }

        //    // --------------------------------------------------------------------
        //    // 4. XỬ LÝ THANH TOÁN (QUAN TRỌNG - ĐÂY LÀ MÔ PHỎNG)
        //    // --------------------------------------------------------------------
        //    _logger.LogInformation("UpgradeToPremium: Attempting payment processing for User {UserId}.", userId.Value);
        //    bool paymentSucceeded = await SimulatePaymentProcessingAsync(user.UserId, cancellationToken);
        //    // Trong ứng dụng thực tế:
        //    // var paymentResult = await _paymentService.ProcessPremiumSubscription(user.UserId, /* payment details */);
        //    // paymentSucceeded = paymentResult.IsSuccess;

        //    if (!paymentSucceeded)
        //    {
        //        _logger.LogWarning("UpgradeToPremium: Payment processing failed for User {UserId}.", userId.Value);
        //        // Cung cấp thông báo lỗi cụ thể hơn nếu có từ payment service
        //        return Result<string>.Failure("Payment processing failed. Please try again or contact support.", StatusCodes.Status402PaymentRequired); // Hoặc 400/500 tùy lỗi
        //    }
        //    _logger.LogInformation("UpgradeToPremium: Payment successful for User {UserId}.", userId.Value);
        //    // --------------------------------------------------------------------

        //    // 5. Cập nhật trạng thái User trong Domain Entity
        //    try
        //    {
        //        user.SetPremiumStatus(true); // Gọi phương thức domain để thay đổi trạng thái

        //        // 6. Đánh dấu User là đã thay đổi (có thể không cần nếu EF Core tự động theo dõi)
        //        _unitOfWork.Users.Update(user); // Đảm bảo EF Core biết entity này đã thay đổi

        //        // 7. Lưu thay đổi vào Database
        //        await _unitOfWork.SaveChangesAsync(cancellationToken);

        //        _logger.LogInformation("UpgradeToPremium: User {UserId} successfully upgraded to premium.", userId.Value);
        //        // --- Enqueue Email gửi lời chúc mừng ---
        //        try
        //        {
        //            var userMail = new PremiumUpgradeEmailDto(toEmail: user.Email!, userName: user.Name);
        //            var jobId = _backgroundJobClient.Enqueue<IEmailService>(
        //                emailService => emailService.SendPremiumUpgradeEmailAsync(
        //                userMail
        //                )
        //            );
        //            _logger.LogInformation("Enqueued premium upgrade confirmation email job {JobId} for User {UserId}.", jobId, user.UserId);
        //        }
        //        catch (Exception emailEx)
        //        {
        //            // Ghi log lỗi enqueue email nhưng không làm quá trình nâng cấp thất bại
        //            _logger.LogError(emailEx, "Failed to enqueue premium upgrade email for User {UserId}.", user.UserId);
        //        }
        //        // 8. Trả về kết quả thành công
        //        // QUAN TRỌNG: Token hiện tại của người dùng KHÔNG có quyền premium mới.
        //        // Cần thông báo cho client biết điều này.
        //        string successMessage = "Account successfully upgraded to Premium. " +
        //                                "Please re-login or refresh your session to access premium features.";
        //        return Result<string>.Success(successMessage, StatusCodes.Status200OK);

        //    }
        //    catch (DbUpdateException dbEx) // Bắt lỗi cụ thể khi lưu DB
        //    {
        //        _logger.LogError(dbEx, "UpgradeToPremium: Database error occurred while upgrading User {UserId}.", userId.Value);
        //        return Result<string>.Failure("A database error occurred while updating your subscription.", StatusCodes.Status500InternalServerError);
        //    }
        //    catch (Exception ex) // Bắt các lỗi không mong muốn khác
        //    {
        //        _logger.LogError(ex, "UpgradeToPremium: An unexpected error occurred for User {UserId}.", userId.Value);
        //        return Result<string>.Failure("An unexpected error occurred.", StatusCodes.Status500InternalServerError);
        //    }
        //}
        public async Task<IResult<string>> Handle(UpgradeToPremiumCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
            {
                _logger.LogWarning("UpgradeToPremium: Unauthorized attempt detected.");
                return Result<string>.Unauthorized();
            }

            var user = await _unitOfWork.Users.GetByIdAsync(userId.Value, cancellationToken);
            if (user == null)
            {
                _logger.LogError("UpgradeToPremium: User with ID {UserId} not found.", userId.Value);
                return Result<string>.Failure($"User not found.", StatusCodes.Status404NotFound);
            }
            if (user.IsPremium)
            {
                _logger.LogInformation("UpgradeToPremium: User {UserId} is already premium.", userId.Value);
                return Result<string>.Failure("Your account is already premium.", StatusCodes.Status400BadRequest);
            }

            // Kiểm tra trạng thái thanh toán
            bool paymentSucceeded = await VerifyPaymentAsync(userId.Value, request.OrderCode, cancellationToken); // Giả sử command có OrderCode
            if (!paymentSucceeded)
            {
                _logger.LogWarning("UpgradeToPremium: Payment verification failed for User {UserId}.", userId.Value);
                return Result<string>.Failure("Payment not confirmed. Please complete payment or contact support.", StatusCodes.Status402PaymentRequired);
            }

            try
            {
                user.SetPremiumStatus(true);
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("UpgradeToPremium: User {UserId} successfully upgraded to premium.", userId.Value);

                try
                {
                    var userMail = new PremiumUpgradeEmailDto(toEmail: user.Email!, userName: user.Name);
                    var jobId = _backgroundJobClient.Enqueue<IEmailService>(
                        emailService => emailService.SendPremiumUpgradeEmailAsync(userMail)
                    );
                    _logger.LogInformation("Enqueued premium upgrade confirmation email job {JobId} for User {UserId}.", jobId, user.UserId);
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Failed to enqueue premium upgrade email for User {UserId}.", user.UserId);
                }

                string successMessage = "Account successfully upgraded to Premium. " +
                                        "Please re-login or refresh your session to access premium features.";
                return Result<string>.Success(successMessage, StatusCodes.Status200OK);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "UpgradeToPremium: Database error occurred while upgrading User {UserId}.", userId.Value);
                return Result<string>.Failure("A database error occurred while updating your subscription.", StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpgradeToPremium: An unexpected error occurred for User {UserId}.", userId.Value);
                return Result<string>.Failure("An unexpected error occurred.", StatusCodes.Status500InternalServerError);
            }
        }
        private async Task<bool> VerifyPaymentAsync(int userId, string orderCode, CancellationToken cancellationToken)
        {
            var transaction = await _unitOfWork.PaymentTransactions.GetByOrderIdAsync(orderCode, cancellationToken);
            if (transaction == null)
            {
                _logger.LogError("Payment verification: Transaction {OrderCode} not found for User {UserId}.", orderCode, userId);
                return false;
            }
            bool paymentSucceeded = transaction.Status == PaymentStatus.Success;
            _logger.LogDebug("Payment verification for User {UserId}, Order {OrderCode}: {Success}", userId, orderCode, paymentSucceeded);
            return paymentSucceeded;
        }
        // --- Phương thức mô phỏng thanh toán ---
        private async Task<bool> SimulatePaymentProcessingAsync(int userId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Simulating payment for User {UserId}...", userId);
            // Giả lập thời gian xử lý
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

            // Giả lập kết quả thành công/thất bại (ví dụ: thành công 90%?)
            // Random rnd = new Random();
            // bool success = rnd.Next(10) < 9;
            bool success = true; // Giả sử luôn thành công cho ví dụ này

            _logger.LogDebug("Simulated payment result for User {UserId}: {Success}", userId, success);
            return success;
        }
    }
    }
