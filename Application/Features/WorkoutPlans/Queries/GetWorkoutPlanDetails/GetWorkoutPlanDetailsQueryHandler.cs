using Application.Features.WorkoutPlans.Queries.Common;
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
using Domain.Enums;

namespace Application.Features.WorkoutPlans.Queries.GetWorkoutPlanDetails
{

    public class GetWorkoutPlanDetailsQueryHandler : IRequestHandler<GetWorkoutPlanDetailsQuery, IResult<WorkoutPlanDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetWorkoutPlanDetailsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult<WorkoutPlanDetailsDto>> Handle(GetWorkoutPlanDetailsQuery request, CancellationToken cancellationToken)
        {
            // Cần một phương thức trong repo để lấy plan chi tiết nhất
            var workoutPlan = await _unitOfWork.WorkoutPlans.GetByIdWithDetailsAsync(request.PlanId, cancellationToken);

            if (workoutPlan == null)
            {
                return Result<WorkoutPlanDetailsDto>.Failure($"Workout Plan with ID {request.PlanId} not found.", StatusCodes.Status404NotFound);
            }

            // --- Xử lý và Map dữ liệu ---

            // 1. Nhóm các bài tập theo Set (nếu DB chưa có cấu trúc Set)
            // Giả sử WorkoutPlanItem có thuộc tính SetName hoặc SetNumber
            var sets = workoutPlan.Items
                .OrderBy(item => item.SetNumber) // Giả sử có SetNumber
                .ThenBy(item => item.ItemOrder)
                .GroupBy(item => item.SetNumber) // Nhóm theo số set
                .Select(group => new ExerciseSetDto(
                    $"Hiệp {group.Key}", // Tên set: "Hiệp 1", "Hiệp 2",...
                    group.Select(item => new ExerciseInSetDto(
                        item.WorkoutId,
                        item.Workout?.Name ?? "N/A",
                        item.Workout?.ImageUrl,
                        // Tạo giá trị "value" dựa trên reps hoặc duration
                        item.Reps.HasValue ? $"{item.Reps}x" :
                        item.DurationSeconds.HasValue ? $"{item.DurationSeconds / 60:00}:{item.DurationSeconds % 60:00}" : "N/A"
                    )).ToList()
                )).ToList();

            // 2. Lấy danh sách dụng cụ cần thiết (ví dụ)
            // Logic này có thể phức tạp hơn, ví dụ có bảng Equipment riêng
            var youWillNeed = workoutPlan.Items
                .Select(item => item.Workout?.RequiredEquipment) // Giả sử Workout có thuộc tính này
                .Where(equipment => !string.IsNullOrEmpty(equipment))
                .Distinct()
                .ToList();


            var difficultyString = workoutPlan.Difficulty switch
            {
                WorkoutDifficultyLevel.Beginner => "Sơ cấp",
                WorkoutDifficultyLevel.Intermediate => "Trung cấp",
                WorkoutDifficultyLevel.Advanced => "Nâng cao",
                WorkoutDifficultyLevel.AllLevels => "Mọi cấp độ",
                _ => "Chưa xác định"
            };
            // 3. Tạo DTO trả về cuối cùng
            var resultDto = new WorkoutPlanDetailsDto(
                workoutPlan.PlanId,
                workoutPlan.Name,
                workoutPlan.Description,
                difficultyString, // Giả sử WorkoutPlan có thuộc tính này
                workoutPlan.Items.Count, // Tổng số bài tập
                workoutPlan.EstimatedDurationMinutes ?? 0,
                workoutPlan.EstimatedCaloriesBurned ?? 0,
                youWillNeed!,
                sets
            );

            return Result<WorkoutPlanDetailsDto>.Success(resultDto, StatusCodes.Status200OK);
        }
    }
    }
