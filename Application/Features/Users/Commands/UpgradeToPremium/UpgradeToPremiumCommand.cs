﻿using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands.UpgradeToPremium
{
    public record UpgradeToPremiumCommand( string? OrderCode) : IRequest<IResult<string>>;

}
