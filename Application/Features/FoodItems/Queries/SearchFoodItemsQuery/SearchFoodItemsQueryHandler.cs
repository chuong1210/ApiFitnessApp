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

namespace Application.Features.FoodItems.Queries.SearchFoodItemsQuery
{
    public class SearchFoodItemsQueryHandler : IRequestHandler<SearchFoodItemsQuery, PaginatedResult<List<FoodItemDto>>>
    {
        private readonly IUnitOfWork _unitOfWork; // Hoặc DbContext
        private readonly IMapper _mapper;

        public SearchFoodItemsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<List<FoodItemDto>>> Handle(SearchFoodItemsQuery request, CancellationToken cancellationToken)
        {
            int pageNumber = request.PageNumber ?? 1;
            int pageSize = request.PageSize ?? 30;

            var query = _unitOfWork.FoodItems.GetAllQueryable(); // Lấy IQueryable

            // Áp dụng bộ lọc tìm kiếm nếu SearchTerm không rỗng
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var lowerSearchTerm = request.SearchTerm.Trim().ToLower();
                query = query.Where(fi => fi.Name.ToLower().Contains(lowerSearchTerm));
            }

            // Lấy tổng số lượng sau khi lọc
            var totalCount = await query.CountAsync(cancellationToken);

            // Sắp xếp và phân trang
            var items = await query
                .OrderBy(fi => fi.Name) // Sắp xếp
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var itemDtos = _mapper.Map<List<FoodItemDto>>(items);

            return PaginatedResult<List<FoodItemDto>>.Success(itemDtos, totalCount, pageNumber, pageSize);
        }
    }
    }
