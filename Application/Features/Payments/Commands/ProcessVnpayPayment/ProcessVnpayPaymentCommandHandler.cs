using Application.Common.Interfaces;
using Application.Responses.Interfaces;
using Application.Responses;
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

namespace Application.Features.Payments.Commands.ProcessVnpayPayment
{

    public class ProcessVnpayPaymentCommandHandler : IRequestHandler<ProcessVnpayPaymentCommand, IResult<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVnpayService _vnpayService;
        private readonly ILogger<ProcessVnpayPaymentCommandHandler> _logger;
        // private readonly IHubContext<NotificationHub> _hubContext; // Inject Hub nếu dùng SignalR thông báo client

        public ProcessVnpayPaymentCommandHandler(
            IUnitOfWork unitOfWork,
            IVnpayService vnpayService,
            ILogger<ProcessVnpayPaymentCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _vnpayService = vnpayService;
            _logger = logger;
        }

        public async Task<IResult<string>> Handle(ProcessVnpayPaymentCommand request, CancellationToken cancellationToken)
        {
            var vnpayData = request.VnpayData;
            _logger.LogInformation("Processing VNPAY callback/IPN data: {VnpayData}", vnpayData);

            // 1. Lấy SecureHash từ VNPAY data
            var vnpSecureHash = vnpayData.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value.ToString();
            if (string.IsNullOrEmpty(vnpSecureHash))
            {
                _logger.LogError("VNPAY Callback: Missing vnp_SecureHash.");
                return Result<string>.Failure("Invalid VNPAY response: Missing signature.", StatusCodes.Status400BadRequest);
            }

            // 2. Kiểm tra chữ ký (SecureHash)
            bool isValidSignature = _vnpayService.ValidateSignature(vnpayData, vnpSecureHash);
            if (!isValidSignature)
            {
                _logger.LogError("VNPAY Callback: Invalid vnp_SecureHash. Potential tampering detected.");
                return Result<string>.Failure("Invalid VNPAY response: Invalid signature.", StatusCodes.Status400BadRequest);
            }
            _logger.LogInformation("VNPAY Callback: Signature validated successfully.");

            // 3. Lấy các thông tin cần thiết từ VNPAY data
            var vnpOrderId = vnpayData.FirstOrDefault(p => p.Key == "vnp_TxnRef").Value.ToString(); // OrderId của chúng ta
            var vnpResponseCode = vnpayData.FirstOrDefault(p => p.Key == "vnp_ResponseCode").Value.ToString();
            var vnpTransactionNo = vnpayData.FirstOrDefault(p => p.Key == "vnp_TransactionNo").Value.ToString(); // Mã GD của VNPAY
            var vnpAmountString = vnpayData.FirstOrDefault(p => p.Key == "vnp_Amount").Value.ToString();
            var vnpBankCode = vnpayData.FirstOrDefault(p => p.Key == "vnp_BankCode").Value.ToString();

            // --- Input validation ---
            if (string.IsNullOrEmpty(vnpOrderId) || string.IsNullOrEmpty(vnpResponseCode) || string.IsNullOrEmpty(vnpAmountString))
            {
                _logger.LogError("VNPAY Callback: Missing required parameters (vnp_TxnRef, vnp_ResponseCode, vnp_Amount).");
                return Result<string>.Failure("Invalid VNPAY response: Missing required parameters.", StatusCodes.Status400BadRequest);
            }

            // Chuyển đổi Amount từ VNPAY (đã nhân 100) về decimal
            if (!long.TryParse(vnpAmountString, out long vnpAmountLong))
            {
                _logger.LogError("VNPAY Callback: Invalid vnp_Amount format: {VnpAmount}", vnpAmountString);
                return Result<string>.Failure("Invalid VNPAY response: Invalid amount format.", StatusCodes.Status400BadRequest);
            }
            decimal vnpAmount = vnpAmountLong / 100m;

            // 4. Tìm giao dịch trong DB bằng OrderId (vnp_TxnRef)
            var transaction = await _unitOfWork.PaymentTransactions.GetByOrderIdAsync(vnpOrderId, cancellationToken);

            if (transaction == null)
            {
                _logger.LogError("VNPAY Callback: Transaction with OrderId {OrderId} not found.", vnpOrderId);
                // Không nên báo lỗi cho VNPAY là giao dịch không tồn tại (bảo mật)
                return Result<string>.Failure("Order not found.", StatusCodes.Status404NotFound); // Hoặc 400
            }

            // 5. Kiểm tra trạng thái giao dịch (chỉ xử lý nếu đang Pending) -> Ngăn xử lý lại
            if (transaction.Status != PaymentStatus.Pending)
            {
                _logger.LogWarning("VNPAY Callback: Transaction {OrderId} already processed with status {Status}.", transaction.OrderId, transaction.Status);
                // Nếu đã thành công thì trả về thành công, nếu thất bại thì trả về thất bại
                if (transaction.Status == PaymentStatus.Success)
                    return Result<string>.Success("Payment was already successfully processed.", StatusCodes.Status200OK);
                else
                    return Result<string>.Failure($"Payment was already processed with status: {transaction.Status}", StatusCodes.Status400BadRequest);
            }

            // 6. Kiểm tra số tiền có khớp không
            if (transaction.Amount != vnpAmount)
            {
                _logger.LogError("VNPAY Callback: Amount mismatch for OrderId {OrderId}. Expected: {ExpectedAmount}, Received: {ReceivedAmount}",
                                 transaction.OrderId, transaction.Amount, vnpAmount);
                // Đánh dấu giao dịch là lỗi và không nâng cấp user
                transaction.MarkAsFailed("AmountMismatch"); // Mã lỗi tự định nghĩa
                _unitOfWork.PaymentTransactions.Update(transaction);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<string>.Failure("Payment amount mismatch.", StatusCodes.Status400BadRequest);
            }

            // 7. Xử lý kết quả dựa trên vnp_ResponseCode
            if (vnpResponseCode == "00") // Thành công
            {
                _logger.LogInformation("VNPAY Callback: Payment Success for OrderId {OrderId}, VNP TransactionNo {VnpTransactionNo}", transaction.OrderId, vnpTransactionNo);

                // 7a. Cập nhật trạng thái PaymentTransaction
                transaction.MarkAsSuccess(vnpTransactionNo, vnpResponseCode, vnpBankCode);
                _unitOfWork.PaymentTransactions.Update(transaction);

                // 7b. Tìm và cập nhật trạng thái User thành Premium
                var user = await _unitOfWork.Users.GetByIdAsync(transaction.UserId, cancellationToken);
                if (user != null)
                {
                    if (!user.IsPremium) // Chỉ nâng cấp nếu chưa phải premium
                    {
                        user.SetPremiumStatus(true);
                        _unitOfWork.Users.Update(user);
                        _logger.LogInformation("Upgraded User {UserId} to Premium for OrderId {OrderId}", user.UserId, transaction.OrderId);
                    }
                    else
                    {
                        _logger.LogWarning("User {UserId} was already Premium when processing successful payment for OrderId {OrderId}", user.UserId, transaction.OrderId);
                    }
                }
                else
                {
                    _logger.LogError("Could not find User {UserId} associated with successful Transaction {OrderId}", transaction.UserId, transaction.OrderId);
                    // Nên làm gì ở đây? Ghi log và báo lỗi hệ thống, nhưng vẫn nên coi giao dịch là thành công.
                }

                // 7c. Lưu tất cả thay đổi vào DB
                try
                {
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    // Gửi thông báo tới client qua SignalR (nếu có)
                    // await _hubContext.Clients.User(transaction.UserId.ToString()).SendAsync("PaymentSuccess", transaction.OrderId);
                    return Result<string>.Success($"Payment successful for order {transaction.OrderId}. Account upgraded to Premium.", StatusCodes.Status200OK);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving successful payment/upgrade for OrderId {OrderId}", transaction.OrderId);
                    // Có thể cần logic để xử lý lại hoặc rollback trạng thái transaction nếu lưu lỗi
                    return Result<string>.Failure("Failed to finalize successful payment processing.", StatusCodes.Status500InternalServerError);
                }
            }
            else // Thất bại hoặc bị hủy
            {
                _logger.LogWarning("VNPAY Callback: Payment Failed/Cancelled for OrderId {OrderId}. ResponseCode: {ResponseCode}", transaction.OrderId, vnpResponseCode);

                // Cập nhật trạng thái PaymentTransaction là Failed (hoặc Cancelled tùy mã)
                transaction.MarkAsFailed(vnpResponseCode);
                _unitOfWork.PaymentTransactions.Update(transaction);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Gửi thông báo tới client qua SignalR (nếu có)
                // await _hubContext.Clients.User(transaction.UserId.ToString()).SendAsync("PaymentFailed", transaction.OrderId, vnpResponseCode);

                return Result<string>.Failure($"Payment failed or was cancelled. (Code: {vnpResponseCode})", StatusCodes.Status400BadRequest);
            }
        }
    }
    }
