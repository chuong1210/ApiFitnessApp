using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ScheduledMeals.Commands.DeleteScheduledMeal
{
    public record DeleteScheduledMealCommand(int ScheduleId) : IRequest<IResult<int>>; // Trả về ID đã xóa

}
