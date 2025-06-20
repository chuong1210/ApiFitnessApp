using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VNPay.NetCore;
// Infrastructure/Persistence/VNPay/PremiumUpgradeProcessor.cs
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Infrastructure.Services
{

    public class PremiumUpgradeProcessor : IVNPayProcessor
    {
        // Đặt một hằng số để định danh processor này
        public const string PROCESSOR_TYPE = "PremiumUpgrade";
        private readonly ILogger<PremiumUpgradeProcessor> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        // Processor là Transient, nhưng nó cần các service Scoped (như IUnitOfWork)
        // -> Chúng ta inject IServiceScopeFactory để tạo một scope mới
        public PremiumUpgradeProcessor(ILogger<PremiumUpgradeProcessor> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public string Type => PROCESSOR_TYPE;

        // Xử lý khi VNPAY gọi đến IPN URL
        public async Task ProcessIPN(VNPayResponse response)
        {
            _logger.LogInformation("Processing VNPAY IPN for OrderId: {OrderId}, ResponseCode: {ResponseCode}",
                response.OrderCode, response.VNPData["vnp_ResponseCode"]);

            // Tạo một scope để lấy các service scoped (IUnitOfWork)
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                // Lấy giao dịch từ DB
                var transaction = await unitOfWork.PaymentTransactions.GetByOrderIdAsync(response.OrderCode);
                if (transaction == null)
                {
                    _logger.LogError("IPN: Transaction with OrderId {OrderId} not found.", response.OrderCode);
                    // Cần trả về response cho VNPAY báo lỗi, nhưng thư viện có thể đã xử lý
                    return;
                }

                // Chỉ xử lý nếu giao dịch đang ở trạng thái Pending
                if (transaction.Status != PaymentStatus.Pending)
                {
                    _logger.LogWarning("IPN: Transaction {OrderId} already processed with status {Status}.", transaction.OrderId, transaction.Status);
                    return;
                }

                // Xác thực chữ ký
                if (!response.Result.HasValue)
                {
                    _logger.LogError("IPN: Invalid signature for OrderId {OrderId}.", response.OrderCode);
                    transaction.MarkAsFailed("InvalidSignature");
                    unitOfWork.PaymentTransactions.Update(transaction);
                    await unitOfWork.SaveChangesAsync();
                    return;
                }

                // Kiểm tra số tiền
                if (transaction.Amount != (response.Amount / 100m))
                {
                    _logger.LogError("IPN: Amount mismatch for OrderId {OrderId}.", response.OrderCode);
                    transaction.MarkAsFailed("AmountMismatch");
                    unitOfWork.PaymentTransactions.Update(transaction);
                    await unitOfWork.SaveChangesAsync();
                    return;
                }


                if (response.Result.HasValue) // Giao dịch thành công
                {
                    _logger.LogInformation("IPN: Payment Success for OrderId {OrderId}", response.OrderCode);

                    transaction.MarkAsSuccess(response.VNPData["vnp_TransactionNo"], response.VNPData["vnp_ResponseCode"], response.VNPData["vnp_BankCode"]);
                    unitOfWork.PaymentTransactions.Update(transaction);

                    var user = await unitOfWork.Users.GetByIdAsync(transaction.UserId);
                    if (user != null && !user.IsPremium)
                    {
                        user.SetPremiumStatus(true);
                        unitOfWork.Users.Update(user);
                        _logger.LogInformation("IPN: Upgraded User {UserId} to Premium.", user.UserId);
                    }

                    await unitOfWork.SaveChangesAsync();
                }
                else // Giao dịch thất bại
                {
                    _logger.LogWarning("IPN: Payment Failed for OrderId {OrderId} with ResponseCode {ResponseCode}", response.OrderCode, response.VNPData["vnp_ResponseCode"]);
                    transaction.MarkAsFailed(response.VNPData["vnp_ResponseCode"]);
                    unitOfWork.PaymentTransactions.Update(transaction);
                    await unitOfWork.SaveChangesAsync();
                }
            }
        }

        // Xử lý khi người dùng được redirect về Return URL
        public async Task ProcessReturnURL(VNPayResponse response)
        {
            _logger.LogInformation("Processing VNPAY ReturnURL for OrderId: {OrderId}, Success: {IsSuccess}",
               response.OrderCode, response.Result.HasValue);

            // Ở đây thường không xử lý logic nghiệp vụ quan trọng
            // vì IPN là nguồn đáng tin cậy hơn.
            // Return URL chủ yếu dùng để điều hướng người dùng trên frontend.
            // Thư viện sẽ tự động redirect về URL mà bạn cung cấp khi tạo link.
            // Tạo một scope để lấy các service scoped (IUnitOfWork)
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                // Lấy giao dịch từ DB
                var transaction = await unitOfWork.PaymentTransactions.GetByOrderIdAsync(response.OrderCode);
                if (transaction == null)
                {
                    _logger.LogError("IPN: Transaction with OrderId {OrderId} not found.", response.OrderCode);
                    // Cần trả về response cho VNPAY báo lỗi, nhưng thư viện có thể đã xử lý
                    return;
                }

                // Chỉ xử lý nếu giao dịch đang ở trạng thái Pending
                if (transaction.Status != PaymentStatus.Pending)
                {
                    _logger.LogWarning("IPN: Transaction {OrderId} already processed with status {Status}.", transaction.OrderId, transaction.Status);
                    return;
                }

                // Xác thực chữ ký
                if (!response.Result.HasValue)
                {
                    _logger.LogError("IPN: Invalid signature for OrderId {OrderId}.", response.OrderCode);
                    transaction.MarkAsFailed("InvalidSignature");
                    unitOfWork.PaymentTransactions.Update(transaction);
                    await unitOfWork.SaveChangesAsync();
                    return;
                }

                // Kiểm tra số tiền
                if (transaction.Amount != (response.Amount / 100m))
                {
                    _logger.LogError("IPN: Amount mismatch for OrderId {OrderId}.", response.OrderCode);
                    transaction.MarkAsFailed("AmountMismatch");
                    unitOfWork.PaymentTransactions.Update(transaction);
                    await unitOfWork.SaveChangesAsync();
                    return;
                }


                if (response.Result.HasValue) // Giao dịch thành công
                {
                    _logger.LogInformation("IPN: Payment Success for OrderId {OrderId}", response.OrderCode);

                    transaction.MarkAsSuccess(response.VNPData["vnp_TransactionNo"], response.VNPData["vnp_ResponseCode"], response.VNPData["vnp_BankCode"]);
                    unitOfWork.PaymentTransactions.Update(transaction);

                    var user = await unitOfWork.Users.GetByIdAsync(transaction.UserId);
                    if (user != null && !user.IsPremium)
                    {
                        //user.SetPremiumStatus(true);
                        unitOfWork.Users.Update(user);
                        _logger.LogInformation("IPN: Upgraded User {UserId} to Premium.", user.UserId);
                    }

                    await unitOfWork.SaveChangesAsync();
                }
                else // Giao dịch thất bại
                {
                    _logger.LogWarning("IPN: Payment Failed for OrderId {OrderId} with ResponseCode {ResponseCode}", response.OrderCode, response.VNPData["vnp_ResponseCode"]);
                    transaction.MarkAsFailed(response.VNPData["vnp_ResponseCode"]);
                    unitOfWork.PaymentTransactions.Update(transaction);
                    await unitOfWork.SaveChangesAsync();
                }
            }
        }


    }
}
