using Application.Common.Interfaces;
using Application.Features.Sleep.Queries.Common;
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

namespace Application.Features.Sleep.Queries.GetLatestSleepLog
{
    public class GetLatestSleepLogQueryHandler : IRequestHandler<GetLatestSleepLogQuery, IResult<SleepLogDto?>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetLatestSleepLogQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<IResult<SleepLogDto?>> Handle(GetLatestSleepLogQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue) return Result<SleepLogDto?>.Unauthorized();

            var latestSleepLog = await _unitOfWork.SleepLogs.GetAllQueryable()
                .Where(sl => sl.UserId == userId.Value)
                .OrderByDescending(sl => sl.EndTime) // Lấy giấc ngủ kết thúc gần nhất
                .ProjectTo<SleepLogDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (latestSleepLog == null)
            {
                // Không có lỗi, chỉ là không có dữ liệu
                return Result<SleepLogDto?>.Success(null, StatusCodes.Status204NoContent);
            }

            return Result<SleepLogDto?>.Success(latestSleepLog, StatusCodes.Status200OK);
        }
    }
    }
