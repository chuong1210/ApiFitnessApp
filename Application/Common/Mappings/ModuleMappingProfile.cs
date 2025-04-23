using Application.Features.Users.Queries.GetUserById;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Mappings
{
    public class ModuleMappingProfile : Profile
    {
        public ModuleMappingProfile()
        {
            // Mapping từ User Entity sang UserDto
            CreateMap<User, UserDto>();

            // Thêm các mapping khác cho các features khác ở đây
            // CreateMap<Workout, WorkoutDto>();
            // ...
        }
    }
}
