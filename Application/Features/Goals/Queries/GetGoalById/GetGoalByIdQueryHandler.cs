using Application.Common.Interfaces;
using Application.Features.Goals.Dtos;
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

namespace Application.Features.Goals.Queries.GetGoalById
{
    public class GetGoalByIdQueryHandler : IRequestHandler<GetGoalByIdQuery, IResult<GoalDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService; // Để kiểm tra quyền sở hữu

        public GetGoalByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<IResult<GoalDto>> Handle(GetGoalByIdQuery request, CancellationToken cancellationToken)
        {
            var goal = await _unitOfWork.Goals.GetByIdAsync(request.GoalId, cancellationToken);
            if (goal == null)
            {
                return Result<GoalDto>.Failure($"Goal with ID {request.GoalId} not found.", StatusCodes.Status404NotFound);
            }

            // Kiểm tra quyền sở hữu
            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue || goal.UserId != currentUserId.Value)
            {
                // Có thể trả về 404 để không tiết lộ sự tồn tại, hoặc 403
                return Result<GoalDto>.Forbidden(); // Hoặc NotFound
            }

            var goalDto = _mapper.Map<GoalDto>(goal);
            return Result<GoalDto>.Success(goalDto, StatusCodes.Status200OK);
        }
    }
    }
