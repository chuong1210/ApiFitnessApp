using Application.Features.Auth.Dtos;
using Application.Responses.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.VerifyOtp
{
    public record VerifyOtpCommand(
        int UserId, // ID của user cần xác thực
        string OtpCode // Mã OTP người dùng nhập
        ) : IRequest<IResult<LoginResponseDto>>; // Trả về LoginResponse (có token) nếu thành công, hoặc null data
}
