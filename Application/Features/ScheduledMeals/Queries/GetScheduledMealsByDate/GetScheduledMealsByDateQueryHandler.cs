using Application.Common.Interfaces;
using Application.Features.ScheduledMeals.Dtos;
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
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ScheduledMeals.Queries.GetScheduledMealsByDate
{

    public class GetScheduledMealsByDateQueryHandler : IRequestHandler<GetScheduledMealsByDateQuery, IResult<List<ScheduledMealDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper; // Cần mapper

        public GetScheduledMealsByDateQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<IResult<List<ScheduledMealDto>>> Handle(GetScheduledMealsByDateQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
            {
                return Result<List<ScheduledMealDto>>.Unauthorized();
            }

          //  var scheduledMeals = await _unitOfWork.ScheduledMeals.GetByUserIdAndDateAsync(userId.Value, request.Date);

            var scheduledMeals = await _unitOfWork.ScheduledMeals
                .GetQueryableByUserIdAndDate(userId.Value, request.Date) // Cần tạo phương thức này
                .Include(sm => sm.PlannedFoodItem) // Lấy thông tin FoodItem
                .OrderBy(sm => sm.MealType) // Sắp xếp theo loại bữa ăn
                .ToListAsync(cancellationToken);

            // Map sang DTO
            var scheduledMealDtos = _mapper.Map<List<ScheduledMealDto>>(scheduledMeals);

            return Result<List<ScheduledMealDto>>.Success(scheduledMealDtos, StatusCodes.Status200OK);
        }
    }
   
}
