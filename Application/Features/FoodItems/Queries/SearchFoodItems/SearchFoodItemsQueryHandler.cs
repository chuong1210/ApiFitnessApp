using Application.Responses.Dtos;
using Application.Responses;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.FoodItems.Queries.SearchFoodItems
{

    public class SearchFoodItemsQueryHandler : IRequestHandler<SearchFoodItemsQuery, PaginatedResult<List<FoodItemDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SearchFoodItemsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<List<FoodItemDto>>> Handle(SearchFoodItemsQuery request, CancellationToken cancellationToken)
        {
            // Gán giá trị mặc định cho phân trang
            int pageNumber = request.PageNumber ?? 1;
            int pageSize = request.PageSize ?? 30;

            // Bắt đầu query từ repository hoặc DbContext (dùng AsNoTracking để tối ưu cho query chỉ đọc)
            var query = _unitOfWork.FoodItems.GetAllQueryable(); // Giả sử phương thức này trả về IQueryable<FoodItem>.AsNoTracking()

            // Áp dụng bộ lọc tìm kiếm theo SearchTerm
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var lowerSearchTerm = request.SearchTerm.Trim().ToLower();
                // Sử dụng EF.Functions.Like nếu cần tìm kiếm phức tạp hơn hoặc không phân biệt chữ hoa/thường ở mức DB
                query = query.Where(fi => fi.Name.ToLower().Contains(lowerSearchTerm));
            }

            // Áp dụng bộ lọc theo Category
            if (!string.IsNullOrWhiteSpace(request.Category))
            {
                query = query.Where(fi => fi.Category != null && fi.Category.ToLower() == request.Category.Trim().ToLower());
            }

            // Lấy tổng số lượng bản ghi (sau khi đã áp dụng tất cả các bộ lọc)
            var totalCount = await query.CountAsync(cancellationToken);

            // Sắp xếp và thực hiện phân trang
            var items = await query
                .OrderBy(fi => fi.Name) // Sắp xếp theo tên A-Z
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<FoodItemDto>(_mapper.ConfigurationProvider) // Dùng ProjectTo để EF Core chỉ query các cột cần thiết cho DTO
                .ToListAsync(cancellationToken);

            // Tạo và trả về kết quả phân trang
            return PaginatedResult<List<FoodItemDto>>.Success(items, totalCount, pageNumber, pageSize);
        }
    }
    }
