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
using Microsoft.EntityFrameworkCore;

namespace Application.Features.FoodItems.Queries.GetPopularFoods
{
    public class GetPopularFoodsQueryHandler : IRequestHandler<GetPopularFoodsQuery, IResult<List<FoodItemDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPopularFoodsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult<List<FoodItemDto>>> Handle(GetPopularFoodsQuery request, CancellationToken cancellationToken)
        {
            // Logic xác định món ăn phổ biến: Dựa trên số lần xuất hiện trong MealLog
            var popularFoodIds = await _unitOfWork.MealLogs.GetQueryable() // Cần phương thức này trong IMealLogRepository
                .GroupBy(ml => ml.FoodId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(request.Count)
                .ToListAsync(cancellationToken);

            if (!popularFoodIds.Any())
            {
                return Result<List<FoodItemDto>>.Success(new List<FoodItemDto>(), StatusCodes.Status200OK);
            }

            // Lấy thông tin chi tiết của các FoodItem phổ biến
            var popularFoods = await _unitOfWork.FoodItems.GetAllQueryable()
                .Where(fi => popularFoodIds.Contains(fi.FoodId))
                .ToListAsync(cancellationToken);

            // Sắp xếp lại theo đúng thứ tự phổ biến
            var sortedPopularFoods = popularFoods
                .OrderBy(fi => popularFoodIds.IndexOf(fi.FoodId))
                .ToList();

            var dtos = _mapper.Map<List<FoodItemDto>>(sortedPopularFoods);
            return Result<List<FoodItemDto>>.Success(dtos, StatusCodes.Status200OK);
        }
    }
    }
