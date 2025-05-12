using Application.Common.Interfaces;
using Application.Features.Goals.Dtos;
using Application.Responses;
using AutoMapper;
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

namespace Application.Features.Goals.Queries.GetAllUserGoals
{
    public class GetAllUserGoalsQueryHandler : IRequestHandler<GetAllUserGoalsQuery, PaginatedResult<List<GoalDto>>>
    {
        private readonly IUnitOfWork _unitOfWork; // Hoặc inject IGoalRepository nếu chỉ cần nó
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllUserGoalsQueryHandler> _logger;


        public GetAllUserGoalsQueryHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetAllUserGoalsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<List<GoalDto>>> Handle(GetAllUserGoalsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
            {
                // Trả về lỗi hoặc kết quả rỗng tùy theo yêu cầu
                _logger.LogWarning("Attempt to get all goals for unauthenticated user.");
                // Sử dụng PaginatedResult.Failure để trả về cấu trúc lỗi nhất quán
                return PaginatedResult<List<GoalDto>>.Failure(StatusCodes.Status401Unauthorized, new List<string> { "User is not authenticated." });
            }

            _logger.LogInformation("Fetching all goals for User ID {UserId}, PageNumber: {PageNumber}, PageSize: {PageSize}",
                userId.Value, request.PageNumber, request.PageSize);

            int pageNumber = request.PageNumber ?? 1;
            int pageSize = request.PageSize ?? 20;

            // Bắt đầu query từ repository (hoặc DbContext)
            // Cần một phương thức trong IGoalRepository trả về IQueryable cho user
            var query = _unitOfWork.Goals.GetQueryableByUserId(userId.Value); // Tạo phương thức này

            // (Tùy chọn) Thêm bộ lọc khác nếu có trong request (ví dụ: filterByType)
            // if (request.filterByType.HasValue)
            // {
            //    query = query.Where(g => g.GoalType == request.filterByType.Value);
            // }

            // Lấy tổng số lượng bản ghi (sau khi lọc nếu có, trước khi phân trang)
            var totalCount = await query.CountAsync(cancellationToken);

            // Sắp xếp (ví dụ: active trước, sau đó theo ngày tạo giảm dần)
            var items = await query
                .OrderByDescending(g => g.IsActive) // Mục tiêu active lên đầu
                .ThenByDescending(g => g.CreatedAt) // Sau đó theo ngày tạo mới nhất
                                                    // Hoặc .OrderByDescending(g => g.StartDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            // Map danh sách kết quả sang DTO
            var goalDtos = _mapper.Map<List<GoalDto>>(items);

            _logger.LogInformation("Successfully retrieved {Count} goals out of {TotalCount} for User ID {UserId}",
                goalDtos.Count, totalCount, userId.Value);

            // Tạo và trả về kết quả phân trang
            return PaginatedResult<List<GoalDto>>.Success(goalDtos, totalCount, pageNumber, pageSize);
        }
    }
}
