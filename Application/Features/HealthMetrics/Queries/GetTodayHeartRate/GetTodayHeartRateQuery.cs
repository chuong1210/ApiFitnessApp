using Application.Features.HealthMetrics.Queries.Common;
using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.HealthMetrics.Queries.GetTodayHeartRate
{
    public record GetTodayHeartRateQuery() : IRequest<IResult<List<HeartRateDataPointDto>>>;

}
