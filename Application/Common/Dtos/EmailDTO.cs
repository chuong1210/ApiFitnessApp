using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public record EmailDto(
     string toEmail,
     string? subject,
     string otpCode
 );
}
