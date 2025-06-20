using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.WorkoutSessions.Queries.Common
{
    public record WorkoutDataPointDto(
        string Date, // "YYYY-MM-DD"
        double Value // Giá trị (ví dụ: Calo đốt cháy)
    );
}
