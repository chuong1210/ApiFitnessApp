using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IAuditableEntity
    {
        DateTime? CreatedAt { get; set; }
        string? CreatedBy { get; set; } // Lưu UserId hoặc Username
        DateTime? UpdatedAt { get; set; }
        string? UpdatedBy { get; set; } // Lưu UserId hoặc Username
    }
}
