using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class PaymentTransactionRepository : IPaymentTransactionRepository
    {
        private readonly AppDbContext _context;
        public PaymentTransactionRepository(AppDbContext context) { _context = context; }

        public async Task<PaymentTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            await _context.PaymentTransactions.FindAsync(new object[] { id }, cancellationToken);

        public async Task<PaymentTransaction?> GetByOrderIdAsync(string orderId, CancellationToken cancellationToken = default) =>
            await _context.PaymentTransactions.FirstOrDefaultAsync(pt => pt.OrderId == orderId, cancellationToken);

        public async Task AddAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default) =>
            await _context.PaymentTransactions.AddAsync(transaction, cancellationToken);

        public void Update(PaymentTransaction transaction) =>
            _context.PaymentTransactions.Update(transaction);
    }

}
