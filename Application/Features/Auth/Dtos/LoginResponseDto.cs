using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Responses.Dtos;
namespace Application.Features.Auth.Dtos
{
    // DTO trả về khi đăng nhập thành công
    public record LoginResponseDto(
        string Token,      // JWT Token
        DateTime ExpiresAt, // Thời gian hết hạn token
                           // Có thể thêm thông tin UserDto nếu muốn
             UserDto? UserDetails
    );
}
