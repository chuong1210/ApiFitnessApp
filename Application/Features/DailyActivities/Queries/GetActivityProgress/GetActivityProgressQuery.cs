using Application.Features.Nutrition.Queries.GetWeeklyNutritionSummary;
using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.DailyActivities.Queries.GetActivityProgress
{
    public record GetActivityProgressQuery(string Period) : IRequest<IResult<List<NutritionDataPointDto>>>;
}
