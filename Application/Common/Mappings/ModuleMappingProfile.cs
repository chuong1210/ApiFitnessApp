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
namespace Application.Common.Mappings
{
    public class ModuleMappingProfile : Profile
    {
        public ModuleMappingProfile()
        {
            // Mapping từ User Entity sang UserDto
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<RegisterUserCommand, User>().ReverseMap();
            CreateMap<RegisterRequestDto, RegisterUserCommand>().ReverseMap();

            CreateMap<FoodItem, FoodItemDto>().ReverseMap();



            // Thêm các mapping khác cho các features khác ở đây
            // CreateMap<Workout, WorkoutDto>();
            // ...
        }
    }
}
