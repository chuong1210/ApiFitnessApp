using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Dtos
{
    public record LoginRequestDto(
        [Required][EmailAddress] string Email,
        [Required] string Password
    );

}
