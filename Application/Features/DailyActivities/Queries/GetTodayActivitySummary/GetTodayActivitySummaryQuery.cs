using Application.Features.DailyActivities.Queries.Common;
using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.DailyActivities.Queries.GetTodayActivitySummary
{
    public record GetTodayActivitySummaryQuery() : IRequest<IResult<DailyActivitySummaryDto>>;

}
