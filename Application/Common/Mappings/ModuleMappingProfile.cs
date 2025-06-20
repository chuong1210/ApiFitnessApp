using Application.Features.Auth.Commands.Register;
using Application.Responses.Dtos;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.Auth.Dtos;
using Application.Features.ScheduledMeals.Dtos;
using Application.Features.Goals.Dtos;
using Application.Features.Workouts.Queries.Common;
using Application.Features.Workouts.Queries.GetWorkoutDetails;
using Application.Features.WorkoutSessions.Queries.Common;
using Application.Features.WorkoutPlans.Queries.GetWorkoutPlanDetails;
using Application.Features.HealthMetrics.Queries.Common;
using Application.Features.Notifications.Queries.Common;
using Application.Features.Sleep.Queries.Common;
namespace Application.Common.Mappings
{
    public class ModuleMappingProfile : Profile
    {
        public ModuleMappingProfile()
        {
            CreateMap<WorkoutPlan, WorkoutPlanDto>()
                .ForMember(dest => dest.TotalExercises, opt => opt.MapFrom(src => src.Items.Count)) // Tính tổng số bài tập
                .ForMember(dest => dest.TotalMinutes, opt => opt.MapFrom(src => src.EstimatedDurationMinutes ?? 0)); // Lấy thời gian ước tính
            CreateMap<WorkoutSession, ScheduledWorkoutDto>()
                .ForMember(dest => dest.ScheduledWorkoutId, opt => opt.MapFrom(src => src.SessionId))
                .ForMember(dest => dest.WorkoutName, opt => opt.MapFrom(src => src.Workout != null ? src.Workout.Name : "N/A"))
                .ForMember(dest => dest.WorkoutId, opt => opt.MapFrom(src => src.WorkoutId ?? 0))
                .ForMember(dest => dest.WorkoutImageUrl, opt => opt.MapFrom(src => src.Workout != null ? src.Workout.ImageUrl : null)) // Thêm mapping cho ảnh
                .ForMember(dest => dest.ScheduledDateTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            // Mapping từ User Entity sang UserDto
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<RegisterUserCommand, User>().ReverseMap();
            CreateMap<RegisterRequestDto, RegisterUserCommand>().ReverseMap();

            CreateMap<FoodItem, FoodItemDto>().ReverseMap();

            CreateMap<MealLog, MealLogDto>().ReverseMap();

            CreateMap<ScheduledMeal, ScheduledMealDto>();
            CreateMap<Notification, NotificationDto>();

            CreateMap<Goal, GoalDto>();
            ; CreateMap<Workout, WorkoutDto>(); // Thêm mapping này
            CreateMap<Workout, WorkoutDetailsDto>(); // Mapping chính
            CreateMap<WorkoutStep, WorkoutStepDto>(); // Mapping cho các bước lồng nhau
            CreateMap<WorkoutPlan, WorkoutPlanDetailsDto>()
        .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.Difficulty.ToString())); // Chỉ định rõ ràng nếu cần
            CreateMap<WorkoutSession, ScheduledWorkoutDto>()
                   .ForMember(dest => dest.ScheduledWorkoutId, opt => opt.MapFrom(src => src.SessionId)) // Map SessionId -> ScheduledWorkoutId
                   .ForMember(dest => dest.WorkoutName, opt => opt.MapFrom(src => src.Workout != null ? src.Workout.Name : "N/A")) // Lấy tên từ Workout liên quan
                   .ForMember(dest => dest.WorkoutId, opt => opt.MapFrom(src => src.WorkoutId ?? 0)) // Lấy WorkoutId
                   .ForMember(dest => dest.ScheduledDateTime, opt => opt.MapFrom(src => src.StartTime)) // Map StartTime -> ScheduledDateTime
                   .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString())); // Chuyển Enum thành String

            CreateMap<HeartRateLog, HeartRateDataPointDto>()
    .ForMember(dest => dest.Bpm, opt => opt.MapFrom(src => src.Bpm)) // Map trường Bpm
    .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp)); // Map 
            // Application/Common/Mappings/ModuleMappingProfile.cs
            CreateMap<WorkoutSession, LatestWorkoutSessionDto>()
                .ForMember(dest => dest.WorkoutName, opt => opt.MapFrom(
                    src => src.Workout != null ? src.Workout.Name : (src.Workout != null ? src.Workout.Name : "Bài tập đã hoàn thành")))
                .ForMember(dest => dest.WorkoutImageUrl, opt => opt.MapFrom(
                    src => src.Workout != null ? src.Workout.ImageUrl : (src.Workout != null ? src.Workout.ImageUrl : null))); // Giả sử Plan cũng có ImageUrl
            CreateMap<SleepLog, SleepLogDto>();
            // Application/Common/Mappings/ModuleMappingProfile.cs

          CreateMap<SleepSchedule, SleepScheduleDto>()
    .ForMember(dest => dest.Tone, opt => opt.MapFrom(src => src.Tone.ToString()))
    
    // Format thành string HH:mm thay vì DateTime
    .ForMember(dest => dest.Bedtime, opt => opt.MapFrom(src =>
        TimeOnly.FromTimeSpan(src.Bedtime).ToString("HH:mm")
    ))
    .ForMember(dest => dest.AlarmTime, opt => opt.MapFrom(src =>
        TimeOnly.FromTimeSpan(src.AlarmTime).ToString("HH:mm")
    ))
    .ForMember(dest => dest.IdealSleepHours, opt => opt.MapFrom(src =>
        CalculateIdealSleepHours(src.Bedtime, src.AlarmTime)
    ));

// Helper method để tính giờ ngủ lý tưởng


        }
        private static double CalculateIdealSleepHours(TimeSpan bedtime, TimeSpan alarmTime)
        {
            try
            {
                var bedtimeMinutes = bedtime.TotalMinutes;
                var alarmTimeMinutes = alarmTime.TotalMinutes;

                // Nếu giờ báo thức nhỏ hơn giờ đi ngủ, cộng thêm 24 giờ
                if (alarmTimeMinutes < bedtimeMinutes)
                {
                    alarmTimeMinutes += 24 * 60; // 24 hours in minutes
                }

                var sleepMinutes = alarmTimeMinutes - bedtimeMinutes;
                return sleepMinutes / 60.0; // Convert to hours
            }
            catch
            {
                return 8.0; // Default 8 hours if calculation fails
            }
        }
    }

}