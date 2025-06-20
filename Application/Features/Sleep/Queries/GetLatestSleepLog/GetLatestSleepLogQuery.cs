using Application.Features.Sleep.Queries.Common;
using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Sleep.Queries.GetLatestSleepLog
{
    public record GetLatestSleepLogQuery() : IRequest<IResult<SleepLogDto?>>; // Trả về nullable

}
