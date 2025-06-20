using Application.Common.Interfaces;
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

namespace Application.Features.MealLogs.Queries.GetTodayMealLogs
{

    public class GetTodayMealLogsQueryHandler : IRequestHandler<GetTodayMealLogsQuery, IResult<List<MealLogDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;
        private readonly IMapper _mapper;

        public GetTodayMealLogsQueryHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IDateTimeService dateTimeService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
            _mapper = mapper;
        }

        public async Task<IResult<List<MealLogDto>>> Handle(GetTodayMealLogsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
            {
                return Result<List<MealLogDto>>.Unauthorized();
            }

            var today = _dateTimeService.UtcNow.Date;
            var startOfDay = today;
            var endOfDay = today.AddDays(1);

            var todayLogs = await _unitOfWork.MealLogs.GetQueryable()
                .Where(ml => ml.UserId == userId.Value && ml.Timestamp >= startOfDay && ml.Timestamp < endOfDay)
                .Include(ml => ml.FoodItem) // Lấy thông tin FoodItem liên quan
                .OrderBy(ml => ml.Timestamp) // Sắp xếp theo thời gian trong ngày
                .ToListAsync(cancellationToken);

            // Dùng AutoMapper để chuyển đổi List<MealLog> sang List<MealLogDto>
            // Đảm bảo MappingProfile của bạn đã có CreateMap<MealLog, MealLogDto>();
            var dtos = _mapper.Map<List<MealLogDto>>(todayLogs);

            return Result<List<MealLogDto>>.Success(dtos, StatusCodes.Status200OK);
        }
    }
}
