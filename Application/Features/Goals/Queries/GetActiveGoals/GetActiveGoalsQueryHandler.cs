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

namespace Application.Features.Goals.Queries.GetActiveGoals
{
    public class GetActiveGoalsQueryHandler : IRequestHandler<GetActiveGoalsQuery, IResult<List<GoalDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetActiveGoalsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<IResult<List<GoalDto>>> Handle(GetActiveGoalsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
            {
                return Result<List<GoalDto>>.Unauthorized();
            }

            var activeGoals = await _unitOfWork.Goals.GetActiveGoalsByUserIdAsync(userId.Value, cancellationToken);

            var goalDtos = _mapper.Map<List<GoalDto>>(activeGoals);
            return Result<List<GoalDto>>.Success(goalDtos, StatusCodes.Status200OK);
        }
    }
    }
