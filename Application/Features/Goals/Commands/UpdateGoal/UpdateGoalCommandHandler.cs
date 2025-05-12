using Application.Common.Interfaces;
using Application.Features.Goals.Dtos;
using Application.Responses.Interfaces;
using Application.Responses;
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
using Domain.Entities;
using Application.Extensions;
namespace Application.Features.Goals.Commands.UpdateGoal
{

    public class UpdateGoalCommandHandler : IRequestHandler<UpdateGoalCommand, IResult<GoalDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateGoalCommandHandler> _logger;

        public UpdateGoalCommandHandler(
            IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
            IMapper mapper, ILogger<UpdateGoalCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IResult<GoalDto>> Handle(UpdateGoalCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
            {
                return Result<GoalDto>.Unauthorized();
            }

            var goal = await _unitOfWork.Goals.GetByIdAsync(request.GoalId, cancellationToken);
            if (goal == null)
            {
                return Result<GoalDto>.Failure($"Goal with ID {request.GoalId} not found.", StatusCodes.Status404NotFound);
            }

            if (goal.UserId != userId.Value)
            {
                return Result<GoalDto>.Forbidden();
            }

            goal.CopyPropertiesFrom<Goal>(goal);
            // Cập nhật entity (tạo phương thức Update trong Goal entity)
            //goal.UpdateDetails(
            //    request.GoalType,
            //    request.TargetValue,
            //    request.StartDate,
            //    request.EndDate
            //);

            // Cập nhật IsActive nếu được cung cấp
            if (request.IsActive.HasValue)
            {
                if (request.IsActive.Value) goal.Activate();
                else goal.Deactivate();
            }

            try
            {
                _unitOfWork.Goals.Update(goal);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Goal {GoalId} updated successfully by User {UserId}.", request.GoalId, userId.Value);

                var updatedDto = _mapper.Map<GoalDto>(goal);
                return Result<GoalDto>.Success(updatedDto, StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating goal {GoalId} for User {UserId}.", request.GoalId, userId.Value);
                return Result<GoalDto>.Failure($"An error occurred: {ex.Message}", StatusCodes.Status500InternalServerError);
            }
        }
    }
    }
