using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Responses;
using Application.Features.Auth.Dtos;

namespace Application.Features.Auth.Commands.GoogleLogin
{
    public record GoogleLoginCommand(
      string IdToken // ID Token nhận từ frontend
  ) : IRequest<IResult<LoginResponseDto>>; // Trả về JWT của ứng dụng
}
