
using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Contracts.Requests
{
    public record UpdateStatusRequestDto ([Required] ScheduleStatus NewStatus);

}
