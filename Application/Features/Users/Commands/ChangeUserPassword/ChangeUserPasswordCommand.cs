using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands.ChangeUserPassword
{
    public record ChangeUserPasswordCommand(
        int UserId,
        string OldPassword,
        string NewPassword
    ) : IRequest<IResult<Unit>>;
}
