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

    public class FoodItemRepository : IFoodItemRepository
    {
        private readonly AppDbContext _context;

        public FoodItemRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<FoodItem?> GetByIdAsync(int foodId, CancellationToken cancellationToken = default)
        {
            return await _context.FoodItems.FindAsync(new object[] { foodId }, cancellationToken);
        }

        public async Task<FoodItem?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.FoodItems
                                 .FirstOrDefaultAsync(fi => fi.Name.ToLower() == name.ToLower(), cancellationToken);
        }

        public async Task<IEnumerable<FoodItem>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await GetAllQueryable().OrderBy(fi => fi.Name).ToListAsync(cancellationToken);
            //return await _context.FoodItems.OrderBy(fi => fi.Name).ToListAsync(cancellationToken);
        }

        public IQueryable<FoodItem> GetAllQueryable()
        {
            // Sử dụng AsNoTracking() vì IQueryable thường dùng cho các truy vấn
            // phức tạp hơn mà không cần theo dõi thay đổi (cải thiện hiệu năng).
            // Nếu bạn cần tracking, bỏ AsNoTracking() đi.
            return _context.FoodItems.AsNoTracking();
        }
        // -

        public async Task<IEnumerable<FoodItem>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllAsync(cancellationToken);
            }
            var lowerSearchTerm = searchTerm.Trim().ToLower();
            // Sử dụng lại GetAllQueryable
            return await GetAllQueryable()
                                 .Where(fi => fi.Name.ToLower().Contains(lowerSearchTerm))
                                 .OrderBy(fi => fi.Name)
                                 .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<FoodItem>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
        {
            //return await _context.FoodItems
            //                     .Where(fi => fi.Category != null && fi.Category.ToLower() == category.ToLower())
            //                     .OrderBy(fi => fi.Name)
            //                     .ToListAsync(cancellationToken);

            var lowerCategory = category.Trim().ToLower();
            // Sử dụng lại GetAllQueryable
            return await GetAllQueryable()
                                 .Where(fi => fi.Category != null && fi.Category.ToLower() == lowerCategory)
                                 .OrderBy(fi => fi.Name)
                                 .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(FoodItem foodItem, CancellationToken cancellationToken = default)
        {
            await _context.FoodItems.AddAsync(foodItem, cancellationToken);
        }

        public void Update(FoodItem foodItem)
        {
            _context.FoodItems.Update(foodItem);
        }

        public void Remove(FoodItem foodItem)
        {
            _context.FoodItems.Remove(foodItem);
        }
    }
}
