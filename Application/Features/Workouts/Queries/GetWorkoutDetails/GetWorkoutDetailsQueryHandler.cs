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

namespace Application.Features.Workouts.Queries.GetWorkoutDetails
{

    public class GetWorkoutDetailsQueryHandler : IRequestHandler<GetWorkoutDetailsQuery, IResult<WorkoutDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetWorkoutDetailsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult<WorkoutDetailsDto>> Handle(GetWorkoutDetailsQuery request, CancellationToken cancellationToken)
        {
            // Cần một phương thức trong repo để lấy workout kèm theo các bước
            var workout = await _unitOfWork.Workouts.GetByIdWithStepsAsync(request.WorkoutId, cancellationToken);

            if (workout == null)
            {
                return Result<WorkoutDetailsDto>.Failure($"Workout with ID {request.WorkoutId} not found.", StatusCodes.Status404NotFound);
            }

            var workoutDto = _mapper.Map<WorkoutDetailsDto>(workout);
            return Result<WorkoutDetailsDto>.Success(workoutDto, StatusCodes.Status200OK);
        }
    }
    // Cần thêm GetByIdWithStepsAsync vào IWorkoutRepository và implementation
    // Interface: public interface IWorkoutRepository { Task<Workout?> GetByIdWithStepsAsync(int workoutId, CancellationToken ct = default); ... }
    // Implementation:
    // public class WorkoutRepository : IWorkoutRepository {
    //     public async Task<Workout?> GetByIdWithStepsAsync(int workoutId, CancellationToken ct = default)
    //     {
    //         return await _context.Workouts
    //                              .Include(w => w.Steps) // Giả sử Workout entity có collection List<WorkoutStep> Steps
    //                              .FirstOrDefaultAsync(w => w.WorkoutId == workoutId, ct);
    //     }
    //     ...
    // }
   
}
