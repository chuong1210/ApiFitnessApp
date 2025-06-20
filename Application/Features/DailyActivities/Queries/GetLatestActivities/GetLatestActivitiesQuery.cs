using Application.Features.DailyActivities.Queries.Common;
using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.DailyActivities.Queries.GetLatestActivities
{
    public record GetLatestActivitiesQuery(int Count) : IRequest<IResult<List<LatestActivityDto>>>;

}
