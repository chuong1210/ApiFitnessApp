using Application.Features.ScheduledMeals.Dtos;
using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ScheduledMeals.Queries.GetScheduledMealsByDate
{
    public record GetScheduledMealsByDateQuery(
     DateOnly Date // Ngày cần lấy lịch
                   // Có thể thêm PageNumber, PageSize nếu một ngày có quá nhiều lịch
 ) : IRequest<IResult<List<ScheduledMealDto>>>; // Trả về danh sách
}
