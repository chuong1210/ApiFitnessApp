using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IPaymentTransactionRepository
    {
        Task<PaymentTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PaymentTransaction?> GetByOrderIdAsync(string orderId, CancellationToken cancellationToken = default);
        Task AddAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default);
        void Update(PaymentTransaction transaction);

        Task<PaymentTransaction?> FindByOrderIdAsync(string orderId);
    }
}
