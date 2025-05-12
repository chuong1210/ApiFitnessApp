using Application.Common.Interfaces;
using Application.Features.Goals.Dtos;
using Application.Responses.Interfaces;
using Application.Responses;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Goals.Commands.CreateGoal
{

    public class CreateGoalCommandHandler : IRequestHandler<CreateGoalCommand, IResult<GoalDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateGoalCommandHandler> _logger;

        public CreateGoalCommandHandler(
            IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
            IMapper mapper, ILogger<CreateGoalCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IResult<GoalDto>> Handle(CreateGoalCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
            {
                return Result<GoalDto>.Unauthorized();
            }

            // Validator có thể đã kiểm tra, nhưng có thể thêm logic nghiệp vụ phức tạp hơn ở đây
            // Ví dụ: Nếu muốn deactivate tất cả các mục tiêu cùng loại đang active trước khi tạo mục tiêu mới
            // var existingActiveGoals = await _unitOfWork.Goals.GetActiveGoalsByUserIdAndTypeAsync(userId.Value, request.GoalType);
            // foreach(var existingGoal in existingActiveGoals) {
            //     existingGoal.Deactivate();
            //     _unitOfWork.Goals.Update(existingGoal);
            // }

            // Tạo Goal entity
            // IsActive mặc định là true khi tạo mới (hoặc tùy bạn quyết định)
            var goal = Goal.Create(
                userId.Value,
                request.GoalType,
                request.TargetValue,
                request.StartDate,
                request.EndDate,
                true // Mặc định là active khi tạo
            );

            try
            {
                await _unitOfWork.Goals.AddAsync(goal, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Goal created successfully for User {UserId}, Type {GoalType}, GoalID {GoalId}",
                    userId.Value, request.GoalType, goal.GoalId);

                var goalDto = _mapper.Map<GoalDto>(goal);
                return Result<GoalDto>.Success(goalDto, StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating goal for User {UserId}, Type {GoalType}.", userId.Value, request.GoalType);
                return Result<GoalDto>.Failure($"An error occurred: {ex.Message}", StatusCodes.Status500InternalServerError);
            }
        }
    }
    }
