﻿using Application.Responses.Dtos;
using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.MealLogs.Queries.GetTodayMealLogs
{
    public record GetTodayMealLogsQuery() : IRequest<IResult<List<MealLogDto>>>;

}
