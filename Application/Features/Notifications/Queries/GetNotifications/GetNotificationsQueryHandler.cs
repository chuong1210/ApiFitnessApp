using Application.Common.Interfaces;
using Application.Features.Notifications.Queries.Common;
using Application.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Notifications.Queries.GetNotifications
{
    public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, PaginatedResult<List<NotificationDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetNotificationsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<List<NotificationDto>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
                return PaginatedResult<List<NotificationDto>>.Failure(StatusCodes.Status401Unauthorized, new List<string> { "Unauthorized" });

            var query = _unitOfWork.Notifications.GetQueryable()
                .Where(n => n.UserId == userId.Value)
                .OrderByDescending(n => n.CreatedAt); // Mới nhất lên đầu

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ProjectTo<NotificationDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return PaginatedResult<List<NotificationDto>>.Success(items, totalCount, request.PageNumber, request.PageSize);
        }
    }
    }
