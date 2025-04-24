using Application.Responses.Dtos;
using Application.Responses;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.FoodItems.Queries.GetAllFoodItems
{
    public class GetAllFoodItemsQueryHandler : IRequestHandler<GetAllFoodItemsQuery, PaginatedResult<List<FoodItemDto>>>
    {
        private readonly IUnitOfWork _unitOfWork; // Chỉ đọc nên có thể inject trực tiếp DbContext nếu muốn
        private readonly IMapper _mapper;

        public GetAllFoodItemsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<List<FoodItemDto>>> Handle(GetAllFoodItemsQuery request, CancellationToken cancellationToken)
        {
            // Thiết lập giá trị mặc định nếu null
            int pageNumber = request.PageNumber ?? 1;
            int pageSize = request.PageSize ?? 30;

            // Bắt đầu query từ repository (hoặc DbContext)
            var query = _unitOfWork.FoodItems.GetAllQueryable(); // GetAllAsync Giả sử có phương thức này trả về IQueryable

            // --- (Tùy chọn) Thêm bộ lọc và sắp xếp ---
            // if (!string.IsNullOrEmpty(request.CategoryFilter))
            // {
            //     query = query.Where(fi => fi.Category != null && fi.Category.ToLower() == request.CategoryFilter.ToLower());
            // }
            // query = query.OrderBy(fi => fi.Name); // Sắp xếp mặc định theo tên

            // Lấy tổng số lượng bản ghi (trước khi phân trang)
            var totalCount = await query.CountAsync(cancellationToken);

            // Phân trang
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            // Map danh sách kết quả sang DTO
            var itemDtos = _mapper.Map<List<FoodItemDto>>(items);

            // Tạo và trả về kết quả phân trang
            return PaginatedResult<List<FoodItemDto>>.Success(itemDtos, totalCount, pageNumber, pageSize);
        }
    }



}
