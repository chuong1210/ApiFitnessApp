using Application.Responses.Dtos;
using Application.Responses.Interfaces;
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
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.FoodItems.Queries.GetRecommendedFoods
{
    public class GetRecommendedFoodsQueryHandler : IRequestHandler<GetRecommendedFoodsQuery, IResult<List<FoodItemDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetRecommendedFoodsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult<List<FoodItemDto>>> Handle(GetRecommendedFoodsQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.FoodItems.GetAllQueryable();

            // Logic gợi ý đơn giản: Lấy các món có calo thấp nhất
            // Bạn có thể thay thế bằng logic phức tạp hơn
            query = query.OrderBy(fi => fi.CaloriesPerServing);

            // (Tùy chọn) Lọc theo loại bữa ăn nếu có
            // Cần có cơ chế map MealType từ String -> các Category phù hợp
            if (!string.IsNullOrEmpty(request.MealType))
            {
                // Ví dụ đơn giản
                if (request.MealType == "Bữa Sáng")
                {
                    query = query.Where(fi => fi.Category == "Breakfast" || fi.Category == "Cake");
                }
            }

            var recommendedFoods = await query
                .Take(request.Count)
                .ProjectTo<FoodItemDto>(_mapper.ConfigurationProvider) // Dùng ProjectTo để tối ưu query
                .ToListAsync(cancellationToken);

            return Result<List<FoodItemDto>>.Success(recommendedFoods, StatusCodes.Status200OK);
        }
    }
}
