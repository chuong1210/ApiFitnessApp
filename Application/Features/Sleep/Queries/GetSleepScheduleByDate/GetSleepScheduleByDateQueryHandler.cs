using Application.Common.Interfaces;
using Application.Features.ScheduledMeals.Commands.UpdateScheduledMealStatus;
using Application.Features.Sleep.Queries.Common;
using Application.Responses;
using Application.Responses.Interfaces;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Sleep.Queries.GetSleepScheduleByDate
{


    public class GetSleepScheduleByDateQueryHandler : IRequestHandler<GetSleepScheduleByDateQuery, IResult<SleepScheduleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetSleepScheduleByDateQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<IResult<SleepScheduleDto>> Handle(GetSleepScheduleByDateQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue) return Result<SleepScheduleDto>.Unauthorized();

            var scheduleEntity = await _unitOfWork.SleepSchedules.GetByUserIdAndDateAsync(
                userId.Value,
                DateOnly.FromDateTime(request.Date),
                cancellationToken);

            if (scheduleEntity == null)
            {
                // Nếu không có lịch trình cụ thể cho ngày đó, có thể trả về lịch trình mặc định của user hoặc 404
                return Result<SleepScheduleDto>.Failure("No sleep schedule found for this date.", StatusCodes.Status404NotFound);
            }

            // Cần tạo mapping từ SleepSchedule (với TimeSpan) sang SleepScheduleDto (với DateTime)
            var dto = _mapper.Map<SleepScheduleDto>(scheduleEntity);

            return Result<SleepScheduleDto>.Success(dto, StatusCodes.Status200OK);
        }
    }
    }
