using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Sleep.Commands.CreateOrUpdateSleepSchedule
{
    public record AddOrUpdateSleepScheduleCommand(
        DateOnly ScheduleDate,
        TimeSpan Bedtime,
        TimeSpan AlarmTime,
        bool IsActive
    // Thêm AlarmTone nếu cần
    ) : IRequest<IResult<int>>; // Trả về ID của lịch trình đã tạo/cập nhật
}
