using Application.Common.Interfaces;
using Application.Features.HealthMetrics.Queries.Common;
using Application.Responses.Interfaces;
using Application.Responses;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.Users.Commands.UpgradeToPremium;
using Hangfire;
using Microsoft.Extensions.Logging;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.HealthMetrics.Queries.GetTodayHeartRate
{
    public class GetTodayHeartRateQueryHandler : IRequestHandler<GetTodayHeartRateQuery, IResult<List<HeartRateDataPointDto>>>
    {
        // ... inject dependencies ...
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        private readonly IDateTimeService _dateTimeService; // Inject dịch vụ thanh toán thật
        private readonly IMapper _mapper; // Inject dịch vụ thanh toán thật


        public GetTodayHeartRateQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, 
            IDateTimeService dateTimeService,IMapper mapper)
        {

            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
            _mapper = mapper;
        }
        public async Task<IResult<List<HeartRateDataPointDto>>> Handle(GetTodayHeartRateQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue) return Result<List<HeartRateDataPointDto>>.Unauthorized();

            var startOfDay = _dateTimeService.UtcNow.Date;
            var endOfDay = startOfDay.AddDays(1);

            // Giả sử có IHeartRateLogRepository
            var heartRateLogs = await _unitOfWork.HeartRateLogs.GetQueryable()
                .Where(hr => hr.UserId == userId.Value && hr.Timestamp >= startOfDay && hr.Timestamp < endOfDay)
                .OrderBy(hr => hr.Timestamp)
                .ProjectTo<HeartRateDataPointDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            // Có thể thêm logic để giảm số lượng điểm dữ liệu trả về nếu quá nhiều
            // Ví dụ: lấy giá trị trung bình mỗi 5 phút

            return Result<List<HeartRateDataPointDto>>.Success(heartRateLogs, StatusCodes.Status200OK);
        }
    }
}
