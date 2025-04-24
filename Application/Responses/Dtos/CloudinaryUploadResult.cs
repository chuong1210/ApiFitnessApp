using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Responses.Dtos
{
    public record CloudinaryUploadResult(bool IsSuccess, string? PublicId, string? Url, string? ErrorMessage);
}
