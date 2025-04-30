using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.ResendOtp
{
    public record ResendOtpCommand(int UserId) : IRequest<IResult<string>>; // Trả về thông báo

}
