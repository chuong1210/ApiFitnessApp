using Application.Common.Interfaces;
using Application.Features.Meals.Queries.GetMealLogHistory;
using Application.Responses.Dtos;
using Application.Responses;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Queries.GetUsers
{

    public class GetMealLogHistoryQueryHandler : IRequestHandler<GetMealLogHistoryQuery, PaginatedResult<List<MealLogDto>>>
    {
        private readonly IUnitOfWork _unitOfWork; // Hoặc DbContext
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper; // Cần mapper để map kết quả

        public GetMealLogHistoryQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<List<MealLogDto>>> Handle(GetMealLogHistoryQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
            {
                // Trả về lỗi hoặc kết quả rỗng tùy theo yêu cầu
                // Ở đây trả về lỗi Unauthorized
                return PaginatedResult<List<MealLogDto>>.Failure(StatusCodes.Status401Unauthorized, new List<string> { "User is not authenticated." });
            }

            int pageNumber = request.PageNumber ?? 1;
            int pageSize = request.PageSize ?? 20;

            // Bắt đầu query (nên dùng IQueryable để tối ưu)
            var query = _unitOfWork.MealLogs.GetQueryableByUserId(userId.Value); // Cần tạo phương thức này

            // Áp dụng bộ lọc ngày tháng
            if (request.StartDate.HasValue)
            {
                var startDateTime = request.StartDate.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(ml => ml.Timestamp >= startDateTime);
            }
            if (request.EndDate.HasValue)
            {
                // Bao gồm cả ngày kết thúc
                var endDateTime = request.EndDate.Value.ToDateTime(TimeOnly.MaxValue); // Hoặc AddDays(1).ToDateTime(TimeOnly.MinValue)
                query = query.Where(ml => ml.Timestamp <= endDateTime);
            }

            // (Tùy chọn) Áp dụng bộ lọc khác (ví dụ: MealType)
            // if (request.mealTypeFilter.HasValue) { query = query.Where(ml => ml.MealType == request.mealTypeFilter.Value); }

            // Sắp xếp (luôn sắp xếp theo thời gian giảm dần)
            query = query.OrderByDescending(ml => ml.Timestamp);

            // Lấy tổng số lượng (sau khi lọc)
            var totalCount = await query.CountAsync(cancellationToken);

            // Phân trang và Include thông tin FoodItem
            var mealLogs = await query
                .Include(ml => ml.FoodItem) // QUAN TRỌNG: Lấy thông tin FoodItem liên quan
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            // Map kết quả sang List<MealLogDto>
            // Cần cấu hình AutoMapper để map MealLog -> MealLogDto (bao gồm cả FoodItem -> FoodItemDto)
            var mealLogDtos = _mapper.Map<List<MealLogDto>>(mealLogs);

            // Tạo và trả về kết quả phân trang
            return PaginatedResult<List<MealLogDto>>.Success(mealLogDtos, totalCount, pageNumber, pageSize);
        }
    }

    // Cần thêm phương thức GetQueryableByUserId vào IMealLogRepository và implementation
    // Interface:
    // public interface IMealLogRepository { IQueryable<MealLog> GetQueryableByUserId(int userId); ... }
    // Implementation:
    // public class MealLogRepository : IMealLogRepository {
    //     public IQueryable<MealLog> GetQueryableByUserId(int userId) =>
    //          _context.MealLogs.Where(ml => ml.UserId == userId).AsNoTracking(); // AsNoTracking vì chỉ đọc
    //     ...
    // }
    // Hoặc inject DbContext vào Handler: _context.MealLogs.Where(...).AsNoTracking()
}
