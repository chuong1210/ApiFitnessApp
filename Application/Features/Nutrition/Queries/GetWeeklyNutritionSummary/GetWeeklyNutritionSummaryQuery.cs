using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Nutrition.Queries.GetWeeklyNutritionSummary
{
    public record NutritionDataPointDto(
        string Date, // "YYYY-MM-DD"
        double Calories
    );

    public record GetWeeklyNutritionSummaryQuery() : IRequest<IResult<List<NutritionDataPointDto>>>;
}
