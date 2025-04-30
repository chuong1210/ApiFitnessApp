using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Responses.Dtos;
namespace Application.Features.Auth.Dtos
{
    // DTO trả về khi đăng nhập thành công
    public record LoginResponseDto
    {
        public string Token { get; init; } = default!;
        public DateTime? ExpiresAt { get; init; }
        public UserDto? UserDetails { get; init; }
    }

}
