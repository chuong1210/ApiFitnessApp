using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands.UpdateUserWeight
{

    public record UpdateUserWeightCommand(
        int UserId,
        double WeightKg
    ) : IRequest<IResult<Unit>>;
}
