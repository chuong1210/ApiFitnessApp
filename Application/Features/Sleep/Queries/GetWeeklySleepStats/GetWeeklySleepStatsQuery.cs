using Application.Features.Sleep.Commands.Common;
using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Sleep.Queries.GetWeeklySleepStats
{
    public record GetWeeklySleepStatsQuery() : IRequest<IResult<List<SleepDataPointDto>>>;

}
